# 1.1 Projektbemutató és motiváció

## Bevezetés

A Software Engineering kurzus keretében egy izgalmas gyakorlati projektet fogsz megvalósítani: egy rendeléskezelő rendszert fogsz fejleszteni. Ez a projekt kiváló lehetőséget nyújt számodra, hogy valós üzleti problémára adj informatikai megoldást, miközben elsajátítod a szükséges technikai készségeket.

## A rendeléskezelő rendszer jelentősége a vállalkozások számára

A rendeléskezelő rendszer kritikus fontosságú eszköz minden vállalkozás számára, függetlenül annak méretétől vagy tevékenységi körétől. Egy ilyen szolgáltatás további szolgáltatásokhoz is kapcsolódhat:

1. **Pénzügyi nyilvántartás:** Pontos és naprakész információt szolgáltat a vállalkozás bevételeiről.
2. **Ügyfélkapcsolatok kezelése:** Lehetővé teszi az ügyféladatok rendszerezett tárolását és kezelését.
3. **Készletgazdálkodás:** Támogatja a termékek nyilvántartását.
4. **Döntéstámogatás:** A beérkező rendelések alapján elemzések, kimutatások készíthetők, amelyek segítik az üzleti döntéshozatalt.

## Miért hasznos a projekt számodra?

A rendeléskezelő rendszer fejlesztése során szerzett tapasztalataidat közvetlenül hasznosíthatod a munkaerőpiacon. A projekt több szempontból is motiváló lehet:

1. **Valós üzleti probléma megoldása:** A fejlesztett rendszered közvetlenül kapcsolódik a vállalkozások mindennapi működéséhez.
2. **Portfólió építése:** A projekted referenciamunkaként szolgálhat a jövőbeli álláskeresés során.
3. **Technológiai stack relevanciája:** A használt technológiák (C#, .NET, Entity Framework, MS SQL) széles körben elterjedtek az iparágban.
4. **Komplex készségfejlesztés:** A projekt nem csak programozási, hanem adatbázis-kezelési, felhasználói felület tervezési és üzleti folyamat modellezési készségeidet is fejleszti.
5. **Azonnali visszacsatolás:** A fejlesztés során azonnal láthatod munkád eredményét, ami növeli a motivációdat.

A rendeléskezelő rendszer fejlesztése során nem csak technikai készségeidet fejleszted, hanem betekintést nyersz egy valós üzleti alkalmazás működésébe is. Ez a projekt ideális híd az elméleti tudásod és a gyakorlati alkalmazás között, felkészítve téged a szoftverfejlesztői karrier kihívásaira.

Vágjunk bele együtt ebbe az izgalmas projektbe!

# 1.2 rendeléskezelő rendszer funkcióinak áttekintése

Ebben a fejezetben részletesen áttekintjük a fejlesztendő rendeléskezelő rendszer főbb funkcióit. Ez az áttekintés segít abban, hogy átfogó képet kapj a projektről, és megértsd, milyen komplex feladattal állsz szemben. Ne aggódj, ha most még bonyolultnak tűnik – a kurzus során lépésről lépésre haladva fogod elsajátítani a szükséges ismereteket és készségeket.

## A rendszer által támogatott főbb folyamatok

A rendeléskezelő rendszerünk több kulcsfontosságú üzleti folyamatot fog támogatni. Ezeket most egyenként áttekintjük:

### 1. Termékkezelés

A termékkezelés modul segítségével nyilvántarthatod a vállalkozás által kínált termékeket. Funkciói:

- Új termékek felvitele
- Termékadatok módosítása
- Termékkategóriák kezelése
- Árak és készletinformációk nyilvántartása

Ez a modul alapvető fontosságú a rendelések gyors és pontos összeállításához, valamint a készletgazdálkodás támogatásához.

### 2. Ügyfélkezelés

Az ügyfélkezelés modul lehetővé teszi a vállalkozás számára, hogy hatékonyan kezelje ügyfelei adatait. Főbb funkciói:

- Új ügyfelek rögzítése
- Meglévő ügyféladatok módosítása
- Ügyfelek keresése és szűrése

Ez a modul biztosítja, hogy mindig naprakész információid legyenek az ügyfeleidről, ami elengedhetetlen a sikeres üzletvitelhez és a személyre szabott kiszolgáláshoz.

### 3. Rendeléskezelés

A rendeléskezelés a rendszer központi funkciója. Itt kapcsolódnak össze az ügyfél- és termékadatok a tényleges értékesítési folyamattal. Főbb jellemzői:

- Új rendelés létrehozása kiválasztott ügyfél részére
- Termékek/szolgáltatások hozzáadása a rendeléshez
- Mennyiség és egységár megadása, kedvezmények kezelése
- Automatikus összegzés és ÁFA-számítás

### 4. Kimutatások és jelentések

Ez a modul segít abban, hogy áttekintést nyerj a vállalkozás pénzügyi helyzetéről és teljesítményéről. Funkciói:

- Időszaki bevétel kimutatása
- Népszerű termékek/szolgáltatások listázása
- Ügyfélstatisztikák generálása

A kimutatások és jelentések modul nélkülözhetetlen az informált üzleti döntéshozatalhoz és a hatékony vállalkozásirányításhoz.

## A felhasználói felület vázlatos bemutatása

A rendeléskezelő rendszer felhasználói felülete Windows Forms technológiával készül, ami lehetővé teszi egy könnyen kezelhető, asztali alkalmazás létrehozását. A felület főbb elemei:

1. **Főmenü:** Itt éred el a rendszer fő funkcióit (Ügyfelek, Termékek, Rendelések, Kimutatások).

2. **Ügyfélkezelő űrlap:** Táblázatos formában jeleníti meg az ügyfeleket, keresési és szűrési lehetőségekkel. Új ügyfél felvitelére és meglévő adatok szerkesztésére külön ablak nyílik.

3. **Termékkezelő űrlap:** Hasonló elrendezésű, mint az ügyfélkezelő, de a termékekre vonatkozó adatokkal és funkciókkal.

4. **Rendeléskezelő űrlap:** Komplex űrlap, ahol kiválaszthatod az ügyfelet, hozzáadhatod a termékeket, megadhatod a mennyiségeket. A rendelés tételei táblázatos formában jelennek meg, az összesítés pedig automatikusan frissül.

5. **Kimutatások űrlap:** Itt választhatod ki a kívánt jelentéstípust és az időszakot. A generált kimutatások táblázatos és/vagy grafikus formában jelennek meg, lehetőséget adva az exportálásra is.

## Mit gondolsz?

Fontos, hogy a rendszer ne csak az alapvető funkciókat biztosítsa, hanem valóban hasznos eszköz legyen a felhasználók számára. Ezért most arra kérlek, gondolkodj el a következő kérdéseken:

1. Milyen további funkciókkal egészítenéd ki a rendszert?
2. Van-e olyan speciális üzleti folyamat, amit szeretnél, ha a rendszer támogatna?
3. Milyen felhasználói élményt javító elemeket tartanál hasznosnak?

A válaszaidat és ötleteidet oszd meg a következő órán. Ez nem csak a saját projekted fejlesztéséhez ad inspirációt, de segít abban is, hogy megtanuld, hogyan gyűjts és értelmezz felhasználói igényeket egy szoftverfejlesztési projekt során.

Remélem, ez az áttekintés segített abban, hogy átfogó képet kapj a fejlesztendő rendeléskezelő rendszerről. A következő fejezetekben részletesen megismerjük az adatbázis tervezés folyamatát, ami az alapját képezi majd a rendszer működésének.