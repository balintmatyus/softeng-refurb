# 2.5 Rendeléskezelés (Rendeles és Rendeles_tetel táblák kezelése)

Ebben a fejezetben egy komplex rendeléskezelő felületet fogunk létrehozni, amely lehetővé teszi a felhasználók számára a rendelések kezelését, beleértve az ügyfelek kiválasztását, rendelések létrehozását, tételek hozzáadását és a rendelések módosítását. Ez a feladat kiváló lehetőséget nyújt arra, hogy az eddig tanult ismereteket egy összetettebb alkalmazásban használjuk fel.

## 1. A RendelesForm létrehozása és felépítése

Első lépésként hozzuk létre a rendeléskezelő űrlapunkat:

1. A Visual Studio Solution Explorer-ében kattints jobb gombbal a projektre, majd válaszd az "Add" > "New Item" menüpontot.
2. A megjelenő ablakban válaszd a "Windows Form" opciót, és nevezd el "RendelesForm"-nak.
3. Kattints az "Add" gombra.

Most, hogy létrehoztuk az új űrlapot, adjuk hozzá a szükséges vezérlőelemeket. A rendeléskezelő felületünk a következő főbb részekből fog állni:

- Ügyfél kiválasztása és szűrése
- Rendelések listázása
- Rendelés részleteinek megjelenítése és szerkesztése
- Termékek listázása és hozzáadása a rendeléshez

Helyezd el a következő vezérlőelemeket az űrlapon:

- 3 db ListBox: ügyfelek, rendelések és termékek listázásához
- 1 db TextBox: ügyfelek szűréséhez
- 1 db DataGridView: rendeléstételek megjelenítéséhez
- ComboBox-ok: szállítási cím és rendelés státuszának kiválasztásához
- TextBox-ok: kedvezmény és mennyiség megadásához
- Gombok: új rendelés létrehozásához, tétel hozzáadásához, törléshez, mentéshez

Az űrlap tervezett kinézete valahogy így nézhet ki:

![Screenshot](./05-img/image.png)

## 2. Adatbázis kapcsolat létrehozása

A rendeléskezelő űrlapunknak szüksége lesz az adatbázishoz való kapcsolódásra. Ehhez használjuk az Entity Framework Core által generált `RendelesDbContext` osztályt.

Adj hozzá egy privát mezőt az osztályodhoz a `DbContext` példány tárolására:

```csharp
public partial class RendelesForm : Form
{
    private readonly RendelesDbContext _context;
    private const decimal AFA = .27m;

    private Ugyfel? KivalasztottUgyfel => lbUgyfelek.SelectedItem as Ugyfel;
    private Cim? KivalasztottCim => cbCimek.SelectedItem as Cim;
    private Rendeles? KivalasztottRendeles => lbRendeles.SelectedItem as Rendeles;
    private Termek? KivalasztottTermek => lbTermek.SelectedItem as Termek;

    public RendelesForm()
    {
        InitializeComponent();
        _context = new RendelesDbContext();
    }

    // ... további kód ...
}
```

A konstruktorban példányosítjuk a `RendelesDbContext`-et. Ez biztosítja, hogy az űrlap teljes életciklusa során rendelkezésünkre álljon az adatbázis-kapcsolat.

## 3. Ügyfelek, címek és termékek listázása

Most, hogy van adatbázis kapcsolatunk, töltsük fel a listáinkat adatokkal. Kezdjük az ügyfelek listázásával!

### 3.1 Ügyfelek listázása és szűrése

Először hozzunk létre egy `BindingSource`-ot az ügyfelekhez. Ezt a legegyszerűbben a tervező nézetben tehetjük meg:

1. Kattints a ListBox vezérlőre, amely az ügyfeleket fogja megjeleníteni.
2. A jobb felső sarokban megjelenő kis nyílra kattintva válaszd a "Choose Data Source" opciót.
3. Az "Add Project Data Source" gombra kattintva hozz létre egy új BindingSource-ot az Ugyfel osztály kiválasztásával.

Most állítsuk be a ListBox tulajdonságait:

- `DataSource`: az imént létrehozott `ugyfelBindingSource`
- `DisplayMember`: "Nev"
- `ValueMember`: "UgyfelId"

Ezután implementáljuk az ügyfelek betöltését és szűrését:

```csharp
private void LoadUgyfelek()
{
    var q = from x in _context.Ugyfel
            where x.Nev.Contains(txtSzuro.Text) || x.Email.Contains(txtSzuro.Text)
            orderby x.Nev
            select x;

    ugyfelBindingSource.DataSource = q.ToList();

    ugyfelBindingSource.ResetCurrentItem();
}
```

A `ResetCurrentItem()` metódus hívása fontos, mert ez biztosítja, hogy a felhasználói felület frissüljön, ha az adatforrás megváltozik. Ez különösen akkor hasznos, a kiválasztott ügyfél alapján szeretnénk további vezérlőket frissíteni.

Ne felejtsd el hozzáadni a szűrő TextBox `TextChanged` eseménykezelőjét:

```csharp
private void txtSzuro_TextChanged(object sender, EventArgs e)
{
    LoadUgyfelek();
}
```

### 3.2 Címek és termékek listázása

A címek és termékek listázása hasonló módon történik. A termékek esetén ugyanúgy létrehozzuk a BindingSource-t, mint az előbbi példában, azonban a címeknél egy lépéssel többre lesz szükség. Ennek oka, hogy a cím részeit képező mezőket össze kellene fűzni egy (számított) mezővé, hiszen egy stringként szeretnénk megjeleníteni a címeket az irányítószámtól a település és közterület nevén keresztül a házszámig.

Ehhez először módosítani fogjuk a `Models` mappában `Cim` osztályunkat a következő sor hozzáadásával:
```csharp
public partial class Cim
{
    [Key]
    [Column("CimID")]
    public int CimId { get; set; }

    [StringLength(100)]
    public string Utca { get; set; } = null!;

    // ... többi mező

    public string CimEgyben => $"{Iranyitoszam}-{Varos}, {Orszag}: {Utca} {Hazszam}";
}
```

Ezután létrehozhatod a BindingSource-okat ezekhez is a tervező nézetben, majd beállíthatod a megfelelő tulajdonságokat:

Címek ComboBox:
- `DataSource`: `cimBindingSource`
- `DisplayMember`: "CimEgyben"
- `ValueMember`: "CimId"

Termékek ListBox:
- `DataSource`: `termekBindingSource`
- `DisplayMember`: "Nev"
- `ValueMember`: "TermekId"

A `Form_Load` eseményben töltsük be az adatokat:

```csharp
private void RendelesForm_Load(object sender, EventArgs e)
{
    LoadData();
    //SetupDataBindings(); // ezt később implementáljuk
}

private void LoadData()
{
    LoadUgyfelek();
    cimBindingSource.DataSource = _context.Cim.ToList();
    termekBindingSource.DataSource = _context.Termek.ToList();
}
```

Ezzel a lépéssel sikeresen létrehoztuk a rendeléskezelő űrlapunk alapjait, és feltöltöttük a szükséges listákat adatokkal. A következő lépésekben a rendelések kezelésével és a felhasználói interakciók implementálásával fogunk foglalkozni.

## 4. Rendelések betöltése

### 4.1 Rendelések ListBox beállítása

Először is, állítsuk be a rendeléseket megjelenítő ListBox tulajdonságait a tervező nézetben:

1. Kattints a rendeléseket megjelenítő ListBox-ra (nevezzük el `lbRendeles`-nek).
2. A Properties ablakban keresd meg a `DataSource` tulajdonságot, és állítsd be egy új BindingSource-ra, amit nevezz el `rendelesBindingSource`-nak.
3. Állítsd be a `DisplayMember` tulajdonságot "RendelesDatum"-ra. Ez fogja biztosítani, hogy a rendelés dátuma jelenjen meg a listában.
4. A `ValueMember` tulajdonságot állítsd "RendelesId"-re.

### 4.2 Rendelések betöltése a kiválasztott ügyfélhez

Most implementáljuk a rendelések betöltését. Ezt egy külön metódusban fogjuk megvalósítani, amit akkor hívunk meg, amikor egy új ügyfelet választanak ki.

Adj hozzá egy új metódust a `RendelesForm` osztályhoz:

```csharp
private void LoadRendelesek()
{
    if (KivalasztottUgyfel == null) return;

    dgvTetelek.DataSource = null;

    var rendeles = from x in _context.Rendeles
                   where x.UgyfelId == KivalasztottUgyfel.UgyfelId
                   select x;

    rendelesBindingSource.DataSource = rendeles.ToList();
    lbRendeles.DataSource = rendelesBindingSource;

    if (lbRendeles.Items.Count > 0)
    {
        lbRendeles.SelectedIndex = 0;
    }

    rendelesBindingSource.ResetCurrentItem();
}
```

Ez a metódus a következőket teszi:

1. Ellenőrzi, hogy van-e kiválasztott ügyfél.
2. Ha van, lekérdezi az adott ügyfélhez tartozó rendeléseket.
3. Beállítja a `rendelesBindingSource` adatforrását a lekérdezett rendelésekre.
4. Frissíti a `lbRendeles` ListBox adatforrását.
5. Ha vannak rendelések, kiválasztja az elsőt.
6. Végül meghívja a `ResetCurrentItem()` metódust, hogy frissítse a felhasználói felületet.

### 4.3 Ügyfél kiválasztásának kezelése

Most hozzá kell adnunk egy eseménykezelőt az ügyfelek ListBox-ához, hogy amikor egy új ügyfelet választanak ki, betöltődjenek a hozzá tartozó rendelések:

```csharp
private void lbUgyfelek_SelectedIndexChanged(object sender, EventArgs e)
{
    LoadRendelesek();
}
```

## 5. Rendeléstételek betöltése DataGridView-ba

### 5.1 DTO objektum létrehozása

Mielőtt a rendeléstételeket betöltenénk a DataGridView-ba, hozzunk létre egy DTO (Data Transfer Object) osztályt.

Hozz létre egy új osztályt `RendelesTetelDTO` néven a `RendelesForm.cs` fájl végén:

```csharp
public class RendelesTetelDTO
{
    public int TetelId { get; set; }
    public string? TermekNev { get; set; }
    public int Mennyiseg { get; set; }
    public decimal EgysegAr { get; set; }
    public decimal Afa { get; set; }
    public decimal NettoAr { get; set; }
    public decimal BruttoAr { get; set; }
}
```

### 5.2 Rendeléstételek betöltése

Most implementáljuk a rendeléstételek betöltését. Adj hozzá egy új metódust a `RendelesForm` osztályhoz:

```csharp
private void LoadRendelesTetel()
{
    if (KivalasztottRendeles == null) return;

    var q = from rt in _context.RendelesTetel
            where rt.RendelesId == KivalasztottRendeles.RendelesId
            select new RendelesTetelDTO
            {
                TetelId = rt.TetelId,
                TermekNev = rt.Termek.Nev,
                Mennyiseg = rt.Mennyiseg,
                EgysegAr = rt.EgysegAr,
                Afa = rt.Afa,
                NettoAr = rt.NettoAr,
                BruttoAr = rt.BruttoAr
            };

    dgvTetelek.DataSource = q.ToList();
}
```

Ez a metódus a következőket teszi:

1. Ellenőrzi, hogy van-e kiválasztott rendelés.
2. Ha van, lekérdezi az adott rendeléshez tartozó tételeket.
3. Az eredményt DTO objektumokká alakítja.
4. Beállítja a DataGridView adatforrását a lekérdezett tételekre.

### 5.3 DataGridView beállítása

Most állítsuk be a DataGridView tulajdonságait, hogy megfeleljen az igényeinknek:

1. Válaszd ki a `dgvTetelek` DataGridView-t a Form Designer-ben.
2. A Properties ablakban állítsd be a következő tulajdonságokat:
   - `AllowUserToAddRows`: False
   - `AllowUserToDeleteRows`: False
   - `ReadOnly`: True
   - `SelectionMode`: FullRowSelect

Ezek a beállítások megakadályozzák, hogy a felhasználó közvetlenül szerkessze a DataGridView tartalmát, és biztosítják, hogy egy egész sort lehessen kiválasztani.

### 5.4 Rendelés kiválasztásának kezelése

Most hozzá kell adnunk egy eseménykezelőt a rendelések ListBox-ához, hogy amikor egy új rendelést választanak ki, betöltődjenek a hozzá tartozó tételek:

```csharp
private void lbRendeles_SelectedIndexChanged(object sender, EventArgs e)
{
    LoadRendelesTetel();
}
```

## 6. Rendelés részleteinek adatkötése

### 6.1 Rendelés címének adatkötése

A rendelés címét már korábban beállítottuk, de most kössük össze a kiválasztott rendeléssel:

1. Válaszd ki a `cbCimek` ComboBox-ot a Form Designer-ben.
2. A Properties ablakban keresd meg a `DataBindings` tulajdonságot.
3. Kattints a "..." gombra a `DataBindings` mellett.
4. Az `SelectedValue` sorban válaszd ki a `rendelesBindingSource`-t, majd a `SzallitasiCimId` tulajdonságot.
5. Az `UpdateMode`-ot állítsd `OnPropertyChanged`-re.

### 6.2 Kedvezmény mértékének adatkötése

Az a célunk, hogy az értékek %-osan jelenjenek meg, azonban a kedvezmények mértéke az adatábizsban törtként, 0-1 közötti számként tároljuk ezt az értéket. Egyszerre szeretnénk a GUI-t megfelelően frissíteni, valamint a mezőbe nem 0-1 közötti értékeket szeretnénk írni, hanem 0-100 közötti %-os értékeket. Ennek megvalósítása kissé komplex, lentebb olvasható a forráskód magyarázattal lépésről lépésre.

```csharp
private void SetupDataBindings()
{
    Binding binding = txtKedvezmeny.DataBindings.Add("Text", rendelesBindingSource, "Kedvezmeny", true, DataSourceUpdateMode.OnPropertyChanged);
    binding.FormattingEnabled = true;
    binding.Format += Binding_Format;
    binding.Parse += Binding_Parse;

    binding.ReadValue();
}
```

1. `Binding binding = txtKedvezmeny.DataBindings.Add(...)`: Ez a sor létrehoz egy új adatkötést a `txtKedvezmeny` TextBox-hoz. 
   - "Text": A TextBox melyik tulajdonságát kötjük (a megjelenített szöveget).
   - `rendelesBindingSource`: Az adatforrás, amihez kötünk.
   - "Kedvezmeny": A forrás melyik tulajdonságához kötünk.
   - `true`: Engedélyezi a formázást.
   - `DataSourceUpdateMode.OnPropertyChanged`: Meghatározza, mikor frissüljön az adatforrás (amikor a tulajdonság értéke változik).

2. `binding.FormattingEnabled = true`: Engedélyezi az egyéni formázást a kötéshez.

3. `binding.Format += Binding_Format`: Hozzárendeli a `Binding_Format` metódust a formázási eseményhez.

4. `binding.Parse += Binding_Parse`: Hozzárendeli a `Binding_Parse` metódust az elemzési eseményhez.

5. `binding.ReadValue()`: Azonnal beolvassa és alkalmazza az értéket a vezérlőelemre.

```csharp
private void Binding_Parse(object? sender, ConvertEventArgs e)
{
    if (e.DesiredType != typeof(decimal)) return;

    string stringValue = e.Value?.ToString()?.Replace("%", "").Trim() ?? "0";
    if (decimal.TryParse(stringValue, out decimal result))
    {
        e.Value = result / 100m;
    }
}
```

Ez a metódus felelős a felhasználó által beírt érték konvertálásáért a megfelelő adattípusra:

1. Ellenőrzi, hogy a kívánt típus decimal-e. Ha nem, kilép a metódusból.
2. Az `e.Value`-ból (ami a TextBox-ba írt szöveg) eltávolítja a % jelet és a felesleges szóközöket.
3. Megpróbálja a stringet decimális számmá konvertálni.
4. Ha sikerül, az eredményt elosztja 100-zal (mivel a kedvezményt tizedesként tároljuk, nem százalékként).

```csharp
private void Binding_Format(object? sender, ConvertEventArgs e)
{
    if (e.DesiredType != typeof(string)) return;

    if (e.Value is decimal value)
    {
        e.Value = value.ToString("P");
    }
}
```

Ez a metódus felelős az adatforrásból érkező érték formázásáért a megjelenítéshez:

1. Ellenőrzi, hogy a kívánt típus string-e. Ha nem, kilép a metódusból.
2. Ha az `e.Value` decimális szám, akkor azt százalékos formátumra alakítja a "P" formázó karakterrel.

Összességében ez a kód lehetővé teszi, hogy a kedvezményt az adatbázisban tizedes törtként tároljuk (pl. 0.1 = 10%), de a felhasználói felületen százalékos formában jelenjen meg és lehessen szerkeszteni (pl. 10%). A `Binding_Parse` metódus gondoskodik arról, hogy a felhasználó által beírt százalékos érték megfelelően konvertálódjon vissza tizedes törtre, míg a `Binding_Format` metódus biztosítja, hogy az adatbázisból kiolvasott tizedes tört százalékos formában jelenjen meg a TextBox-ban.

Ne felejtsd el meghívni a `SetupDataBindings()` metódust a Form konstruktorában vagy a `Load` eseménykezelőjében.

### 6.3 Státusz adatkötése

A státusz ComboBox-ot a következőképpen állítsd be:

1. Válaszd ki a `cbStatusz` ComboBox-ot a Form Designer-ben.
2. A Properties ablakban állítsd be az `Items` tulajdonságot a következő értékekkel:
   - "Feldolgozás alatt"
   - "Szállítás"
   - "Kiszállítva"
   - "Törölve"
3. A `DataBindings` tulajdonságnál add hozzá a `Text` tulajdonságot a `rendelesBindingSource`-hoz, és kösd a `Statusz` mezőhöz.
4. Az `UpdateMode`-ot állítsd `OnPropertyChanged`-re.

## 7. Új rendelés hozzáadása

Most implementáljuk az új rendelés hozzáadását. Ehhez egy új gombot fogunk használni, amit nevezzünk el `btnUjRendeles`-nek.

### 7.1 Új rendelés gomb eseménykezelőjének implementálása

Valósítsd meg a következőket:

1. Ellenőrizd, hogy van-e kiválasztott ügyfél.
2. Hozz létre egy új `Rendeles` objektumot az aktuális dátummal és alapértelmezett értékekkel.
3. Add hozzá az új rendelést az adatbázishoz.
4. Frissítsd a rendelések listáját és válaszd ki az új rendelést.

```csharp
private void btnUjRendeles_Click(object sender, EventArgs e)
{
    if (KivalasztottUgyfel == null)
    {
        return;
    }

    var cim = KivalasztottUgyfel.Lakcim ?? _context.Cim.FirstOrDefault();

    if (cim == null)
    {
        MessageBox.Show("Nincs cím megadva.");
        return;
    }

    var ujRendeles = new Rendeles()
    {
        UgyfelId = KivalasztottUgyfel.UgyfelId,
        SzallitasiCimId = cim.CimId,
        RendelesDatum = DateTime.Now,
        Kedvezmeny = 0,
        Vegosszeg = 0,
        Statusz = "Feldolgozás alatt"
    };

    _context.Rendeles.Add(ujRendeles);
    Mentés();

    lblRendelesLabel.Text = $"A rendelés teljes értéke: {ujRendeles.Vegosszeg} Ft";

    LoadRendelesek();

    lbRendeles.SelectedItem = ujRendeles;
}

private void Mentés()
{
    try
    {
        _context.SaveChanges();
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}
```

## 8. Tétel hozzáadása, törlése és végösszeg számítása

### 8.1 Új tétel hozzáadása

Először implementáljuk az új tétel hozzáadását. Ehhez hozzunk létre egy új gombot `btnHozzaad` néven, és implementáljuk az eseménykezelőjét:

1. Ellenőrizzük, hogy érvényes-e a megadott mennyiség.
2. Ellenőrizzük, hogy ki van-e választva rendelés és termék.
3. Hozzunk létre egy új `RendelesTetel` objektumot a megadott adatokkal.
4. Adjuk hozzá az új tételt az adatbázishoz és mentsük a változásokat.
5. Frissítsük a tételek listáját és a végösszeget. (utóbbi egyelőre kommentelve)

```csharp
private void btnHozzaad_Click(object sender, EventArgs e)
{
    if (!int.TryParse(txtMennyiseg.Text, out int mennyiseg) || mennyiseg <= 0)
    {
        MessageBox.Show("Rossz mennyiség!");
        return;
    }

    if (KivalasztottRendeles == null || KivalasztottTermek == null)
    {
        MessageBox.Show("Nincs kiválasztva rendelés vagy termék!");
        return;
    }

    decimal bruttoAr = KivalasztottTermek.AktualisAr * (1 + AFA);

    var ujTetel = new RendelesTetel
    {
        RendelesId = KivalasztottRendeles.RendelesId,
        TermekId = KivalasztottTermek.TermekId,
        Mennyiseg = mennyiseg,
        EgysegAr = KivalasztottTermek.AktualisAr,
        Afa = AFA,
        NettoAr = KivalasztottTermek.AktualisAr * mennyiseg,
        BruttoAr = bruttoAr
    };

    _context.RendelesTetel.Add(ujTetel);
    Mentés();

    LoadRendelesTetel();

    UpdateVegosszeg();
}
```

### 8.2 Tétel törlése

Most implementáljuk a tétel törlését. Rendeljünk a `btnTorol` gombhoz eseménykezelőt:

```csharp
private void btnTorol_Click(object sender, EventArgs e)
{
    if (dgvTetelek.SelectedRows.Count == 0)
    {
        MessageBox.Show("Nincs kiválasztva tétel!");
        return;
    }

    var selectedTetel = dgvTetelek.SelectedRows[0].DataBoundItem as RendelesTetelDTO;

    var tetel = (from rt in _context.RendelesTetel
                 where rt.TetelId == selectedTetel!.TetelId
                 select rt).FirstOrDefault();

    if (tetel != null)
    {
        _context.RendelesTetel.Remove(tetel);
        Mentés();

        LoadRendelesTetel();
        UpdateVegosszeg();
    }
}
```

Ez a kód a következőket teszi:
1. Ellenőrzi, hogy ki van-e választva tétel a DataGridView-ban.
2. Megkeresi a kiválasztott tételt az adatbázisban.
3. Ha megtalálta, törli a tételt és menti a változásokat.
4. Frissíti a tételek listáját és a végösszeget.

### 8.3 Végösszeg számítása

Most implementáljuk a végösszeg számítását. Hozzunk létre egy új metódust `UpdateVegosszeg` néven:

```csharp
private void UpdateVegosszeg()
{
    if (lbRendeles.SelectedItem == null) return;

    var selectedRendeles = (Rendeles)lbRendeles.SelectedItem;

    var vegosszeg = (from rt in _context.RendelesTetel
                     where rt.RendelesId == selectedRendeles.RendelesId
                     select rt.Mennyiseg * rt.BruttoAr).Sum();

    selectedRendeles.Vegosszeg = vegosszeg * (1 - selectedRendeles.Kedvezmeny);

    Mentes();

    // Frissítjük a felületet
    rendelesBindingSource.ResetBindings(false);
}
```

Ez a metódus a következőket teszi:
1. Ellenőrzi, hogy ki van-e választva rendelés.
2. Kiszámolja a rendelés tételeinek összértékét.
3. Alkalmazza a kedvezményt a végösszegre.
4. Menti a változásokat az adatbázisba.
5. Frissíti a felhasználói felületet.

### 8.4 Felület frissítése

Most frissítsük a felületet, hogy megjelenítse a végösszeget. Adjunk hozzá egy új Label vezérlőt `lblVegosszeg` néven, és kössük össze a rendelés végösszegével:

```csharp
private void SetupDataBindings()
{
    // ... Korábbi kötések ...

    Binding vegosszegBinding = lblVegosszeg.DataBindings.Add("Text", rendelesBindingSource, "Vegosszeg", true);
    vegosszegBinding.FormattingEnabled = true;
    vegosszegBinding.FormatString = "C0"; // Pénznem formátum, nulla tizedesjeggyel
}
```

### 8.5 Események összekapcsolása

Végül, módosítsuk a `lbRendeles_SelectedIndexChanged` eseménykezelőt, hogy frissítse a végösszeget is:

```csharp
private void lbRendeles_SelectedIndexChanged(object sender, EventArgs e)
{
    LoadRendelesTetel();
    UpdateVegosszeg();
}
```

## 9. Mentés funkció implementálása

A mentés funkció kulcsfontosságú egy adatkezelő alkalmazásban. Bár eddig is használtunk mentést egyes műveletek után, most egy explicit mentés funkciót fogunk létrehozni, amely lehetővé teszi a felhasználó számára, hogy bármikor elmenthesse a módosításokat.

### 9.1 Mentés funkció implementálása

Adj hozzá egy új eseménykezelőt a mentés gombhoz:

```csharp
private void btnMentes_Click(object sender, EventArgs e)
{
    Mentés();
}
```

# Összefoglalás

Ebben a fejezetben egy komplex rendeléskezelő alkalmazás fejlesztésének folyamatát mutattuk be Windows Forms és Entity Framework Core használatával. Az alkalmazás lehetővé teszi a felhasználók számára az ügyfelek, rendelések és rendeléstételek kezelését egy intuitív grafikus felületen keresztül.

A fejlesztés során a következő főbb funkciókat valósítottuk meg:

1. Adatbázis kapcsolat létrehozása és kezelése Entity Framework Core segítségével
2. Ügyfelek listázása és szűrése
3. Rendelések megjelenítése és kezelése a kiválasztott ügyfélhez
4. Rendeléstételek kezelése DataGridView segítségével
5. Új rendelés létrehozása
6. Tételek hozzáadása és törlése a rendelésekhez
7. Rendelés részleteinek (cím, kedvezmény, státusz) adatkötése és szerkesztése
8. Végösszeg dinamikus számítása a tételek és a kedvezmény alapján
9. Mentés funkció implementálása változáskövetéssel

Az alkalmazás fejlesztése során számos fontos koncepciót és technikát alkalmaztunk, többek között:

- Data Transfer Object (DTO) használata az adatok biztonságos és hatékony kezelésére
- Adatkötés (data binding) a felhasználói felület és az adatmodell összekapcsolására
- LINQ lekérdezések az adatok hatékony szűrésére és rendezésére
- Eseményvezérelt programozás a felhasználói interakciók kezelésére
- Hibakezelés és felhasználói visszajelzések a megbízható működés érdekében

Ez a projekt kiváló példa arra, hogyan lehet egy összetett üzleti folyamatot (rendeléskezelés) leképezni egy felhasználóbarát szoftveralkalmazásra. A fejlesztés során szem előtt tartottuk a kód újrafelhasználhatóságát, a moduláris felépítést és a megfelelő szintű absztrakciót.

Az alkalmazás további fejlesztési lehetőségeket is rejt magában, például:

- Részletesebb riportok és statisztikák generálása
- Fejlettebb keresési és szűrési lehetőségek implementálása
- Termékkezelés funkciók hozzáadása
- Felhasználói jogosultságok kezelése
- Adatok exportálása különböző formátumokba (pl. Excel)

Ez a projekt kiváló alapot nyújt a Windows Forms alkalmazásfejlesztés és az adatbázis-kezelés gyakorlati elsajátításához, valamint jó kiindulópont lehet további, összetettebb üzleti alkalmazások fejlesztéséhez.