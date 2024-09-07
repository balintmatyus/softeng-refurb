# 2.2 CRUD műveletek és DataBinding TreeView segítségével (Termékkategóriák adatainak kezelése)

Ebben a fejezetben a rendeléskezelő rendszerünk egy fontos részét fogjuk megvalósítani: a termékkategóriák kezelését. Ez a funkció lehetővé teszi, hogy a felhasználók megtekinthessék, szerkeszthessék a termékkategóriákat egy könnyen használható, fa struktúrájú felületen keresztül. A fejlesztés során megismerkedünk a Windows Forms alkalmazások több fontos koncepciójával, beleértve a többablakos alkalmazások létrehozását, a TreeView vezérlő használatát, valamint az adatkötést (data binding).

## A TermekKategoriaForm létrehozása

Először hozzuk létre a TermekKategoriaForm-ot, amely a termékkategóriák kezelésére szolgál.

1. Jobb kattintás a projektre a Solution Explorer-ben, majd "Add" > "New Item".
2. Válaszd a "Windows Form" sablont, nevezd el "TermekKategoriaForm"-nak, majd kattints az "Add" gombra.
3. Tervezd meg a form felületét a következő vezérlőkkel:
   - TreeView a kategóriák megjelenítéséhez
   - Két TextBox a név és leírás szerkesztéséhez
   - Egy Button a mentéshez

4. Állítsd be a vezérlők tulajdonságait:
   - TreeView:
     - Name: treeViewKategoriak
     - Dock: Left
   - TextBox (név):
     - Name: txtNev
   - TextBox (leírás):
     - Name: txtLeiras
     - Multiline: True
   - Button:
     - Name: btnMentes
     - Text: "Mentés"

## A főmenü létrehozása (Form1)

Most hozzuk létre a főmenüt, amely a felhasználói felületünk kiindulópontja lesz. Ez a Form1 nevű űrlap lesz, amelyre egy gombot helyezünk el a termékkategóriák kezelésének elindításához.

1. Nyisd meg a Form1.cs fájlt a Visual Studio tervező nézetében.
2. A Toolbox panelről húzz egy Button vezérlőt az űrlapra.
3. Állítsd be a gomb tulajdonságait a Properties ablakban:
   - Name: btnTermekKategoriak
   - Text: "Termékkategóriák kezelése"

4. Dupla kattintással a gombra hozz létre egy eseménykezelőt, és írd be a következő kódot:

```csharp
private void btnTermekKategoriak_Click(object sender, EventArgs e)
{
    TermekKategoriaForm termekKategoriaForm = new TermekKategoriaForm();
    termekKategoriaForm.ShowDialog();
}
```

Ez a kód létrehoz egy új TermekKategoriaForm példányt és megjeleníti azt modális párbeszédablakként.

## Adatmodell és adatkötés implementálása

Most, hogy elkészítettük a felhasználói felületünket, következő lépésként implementáljuk az adatmodellt és az adatkötést. Ez a folyamat lehetővé teszi, hogy a TreeView-ban megjelenített adatok kapcsolatban legyenek az adatbázissal, és automatikusan frissüljenek a változtatások esetén.

### 1. DbContext példány létrehozása

Először is szükségünk van egy DbContext példányra, amely kapcsolatot teremt az alkalmazásunk és az adatbázis között.

1. Nyisd meg a TermekKategoriaForm.cs fájlt.

2. A form osztályának elején hozz létre egy új privát mezőt a DbContext számára:

```csharp
public partial class TermekKategoriaForm : Form
{
    private RendelesDbContext _context;

    // ... többi kód ...
}
```

3. A form konstruktorában inicializáld a DbContext-et, és hívd meg a kategóriák betöltését végző metódust:

```csharp
public TermekKategoriaForm()
{
    InitializeComponent();
    _context = new RendelesDbContext();
    LoadKategoriak();
}
```

### 2. Kategóriák betöltése

Most implementáljuk a `LoadKategoriak` metódust, amely betölti a kategóriákat az adatbázisból és megjeleníti azokat a TreeView-ban.

```csharp
private void LoadKategoriak()
{
    // Az összes kategória lekérdezése az adatbázisból
    var kategoriak = (from k in _context.TermekKategoria
                      select k).ToList();

    // TreeView tartalmának törlése
    treeViewKategoriak.Nodes.Clear();

    // Főkategóriák hozzáadása a TreeView-hoz
    var fokategoriak = from k in kategoriak
                       where k.SzuloKategoriaId == null
                       select k;

    foreach (var kategoria in fokategoriak)
    {
        var node = CreateTreeNode(kategoria, kategoriak);
        treeViewKategoriak.Nodes.Add(node);
    }
}
```

Ez a metódus a következő lépéseket hajtja végre:
1. Lekérdezi az összes kategóriát az adatbázisból.
2. Törli a TreeView jelenlegi tartalmát.
3. Kiválasztja a főkategóriákat (amelyeknek nincs szülő kategóriájuk).
4. Minden főkategóriához létrehoz egy TreeNode-ot, és hozzáadja a TreeView-hoz.

### 3. TreeNode létrehozása rekurzívan

A `CreateTreeNode` metódus felelős a kategóriák fa struktúrájának felépítéséért. Ez a metódus rekurzívan működik, ami azt jelenti, hogy önmagát hívja meg az alkategóriák létrehozásához.

```csharp
private TreeNode CreateTreeNode(TermekKategoria kategoria, List<TermekKategoria> osszeKategoria)
{
    // TreeNode létrehozása az aktuális kategóriához
    var node = new TreeNode(kategoria.Nev) { Tag = kategoria };

    // Alkategóriák keresése és hozzáadása
    var alkategoriak = from k in osszeKategoria
                       where k.SzuloKategoriaId == kategoria.KategoriaId
                       select k;

    foreach (var gyerekKategoria in alkategoriak)
    {
        node.Nodes.Add(CreateTreeNode(gyerekKategoria, osszeKategoria));
    }

    return node;
}
```

Ez a metódus a következőképpen működik:
1. Létrehoz egy új TreeNode-ot az aktuális kategóriához.
2. A Tag tulajdonságban eltárolja a teljes kategória objektumot, hogy később könnyen hozzáférhessünk.
3. Megkeresi az összes alkategóriát az aktuális kategóriához.
4. Minden alkategóriához rekurzívan meghívja önmagát, és az eredményt hozzáadja az aktuális node gyerekeihez.

### 4. Kiválasztott kategória adatainak megjelenítése

Most, hogy a TreeView-ban megjelennek a kategóriák, implementáljuk azt a funkciót, amely megjeleníti a kiválasztott kategória adatait a TextBox-okban.

1. A TermekKategoriaForm.cs fájlban keresd meg a TreeView vezérlő eseménykezelőjét (ha nincs, hozd létre a tervező nézetben a TreeView AfterSelect eseményéhez).

2. Implementáld az eseménykezelőt:

```csharp
private void treeViewKategoriak_AfterSelect(object sender, TreeViewEventArgs e)
{
    if (e.Node?.Tag is TermekKategoria selectedKategoria)
    {
        txtNev.Text = selectedKategoria.Nev;
        txtLeiras.Text = selectedKategoria.Leiras;
    }
}
```

Ez a kód a következőket csinálja:
1. Ellenőrzi, hogy a kiválasztott csomópont Tag tulajdonsága tartalmaz-e egy TermekKategoria objektumot.
2. Ha igen, akkor beállítja a TextBox-ok szövegét a kategória nevére és leírására.

### 5. Változtatások mentése

Végül implementáljuk a Mentés gomb funkcionalitását, amely elmenti a TextBox-okban végrehajtott változtatásokat az adatbázisba.

1. Keresd meg a Mentés gomb Click eseménykezelőjét (ha nincs, hozd létre a tervező nézetben).

2. Implementáld az eseménykezelőt:

```csharp
private void btnMentes_Click(object sender, EventArgs e)
{
    if (treeViewKategoriak.SelectedNode?.Tag is TermekKategoria selectedKategoria)
    {
        selectedKategoria.Nev = txtNev.Text;
        selectedKategoria.Leiras = txtLeiras.Text;

        _context.SaveChanges();

        // Frissítjük a TreeView-t
        LoadKategoriak();
    }
}
```

Ez a kód a következő lépéseket hajtja végre:
1. Ellenőrzi, hogy van-e kiválasztott kategória.
2. Ha van, frissíti a kategória nevét és leírását a TextBox-ok tartalmával.
3. Elmenti a változtatásokat az adatbázisba a `SaveChanges()` metódussal.
4. Újra betölti a kategóriákat a TreeView-ba, hogy a változtatások azonnal láthatóak legyenek.

Ezzel az implementációval létrehoztunk egy alapvető, de működő termékkategória-kezelő modult. A felhasználók most már meg tudják tekinteni a kategóriákat egy fa struktúrában, kiválaszthatnak egy kategóriát a szerkesztéshez, és el tudják menteni a változtatásaikat.

Ez a megoldás jó kiindulópont, de még van lehetőség további fejlesztésekre, például új kategóriák létrehozására, törlésre, vagy a hierarchia módosítására. Ezeket a funkciókat a későbbiekben adhatod hozzá az alkalmazáshoz.

## 2.2.4 Hibakezelés és validáció

A felhasználói élmény és az adatintegritás javítása érdekében adjunk hozzá néhány alapvető hibakezelési és validációs lépést.
* Ha a beírt kategórianév üres, dobjunk hibaüzenetet.
* Értesítsük a felhasználót a mentés eredményéről.

A Mentés gomb eseménykezelőjében adjunk hozzá validációt:

<details><summary>Megoldás</summary>

```csharp
private void btnMentes_Click(object sender, EventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtNev.Text))
    {
        MessageBox.Show("A név mező nem lehet üres!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    if (treeViewKategoriak.SelectedNode?.Tag is TermekKategoria selectedKategoria)
    {
        try
        {
            selectedKategoria.Nev = txtNev.Text;
            selectedKategoria.Leiras = txtLeiras.Text;

            _context.SaveChanges();

            MessageBox.Show("A változtatások sikeresen mentve!", "Siker", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Frissítjük a TreeView-t
            LoadKategoriak();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hiba történt a mentés során: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

</details><br/>

