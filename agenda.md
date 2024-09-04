## 6 Heti Tanulási Terv: Adatbázis-alapú Windows Forms Alkalmazásfejlesztés (CRUD)

**Cél:** A hallgatók egy gyakorlati projekt (számlázó rendszer) keretében elsajátítják az adatbázison alapuló Windows Forms alkalmazások fejlesztéséhez szükséges ismereteket, beleértve a CRUD műveleteket.

**Fejlesztői környezet:** Visual Studio, C#, .NET 8, Entity Framework (Database-first)

**Előfeltételek:** A hallgatók rendelkeznek alapismeretekkel C# nyelvben, Windows Forms technológiában és MS SQL adatbázisokban (SQL nyelv).

**Megközelítés:** Learning-by-doing, gyakorlatorientált tanulás

### 1. Hét: Projekt Bevezetés és Adatbázis Tervezés

* **Projekt bemutatása:** A számlázó rendszer funkcióinak és céljainak áttekintése.
* **Adatbázis tervezés:** 
    * Entitások (táblák) meghatározása: ügyfél, termék, számla, számla tétel stb.
    * Kapcsolatok meghatározása az entitások között.
    * Adatbázis létrehozása MS SQL Serverben.
    * Entity Framework modell generálása (Database-first).

**1. Projektbemutató és motiváció** 
* A számlázórendszer jelentősége a vállalkozások számára
* A projekt céljainak és elvárt eredményeinek ismertetése
* A hallgatók motiválása a gyakorlatias tanulásra

**2. Számlázórendszer funkcióinak áttekintése** 
* A rendszer által támogatott főbb folyamatok bemutatása (pl. ügyfélkezelés, termékkezelés, számlakészítés)
* A felhasználói felület vázlatos bemutatása
* A hallgatók ötleteinek és elvárásainak összegyűjtése a rendszerrel kapcsolatban

**3. Adatbázis tervezés elméleti alapjai** 
* Az adatbázisok szerepe az alkalmazásokban
* MS-SQL server rövid bemutatása és egyéb potenciális adatbázisszerverek említésszintű felsorolása
* A relációs adatbázisok alapfogalmai (táblák, oszlopok, adattípusok MS-SQL környezetben, elsődleges kulcs)
* Constraintek alkalmazása, default value
* Táblák közötti kapcsolatok, idegen kulcs
* Normalizálás alapelvei példán keresztül

**4. A számlázórendszer adatbázisának tervezése** 
* Táblák (entitások) azonosítása és megnevezése
* Táblák közötti kapcsolatok meghatározása (egy-a-többhöz, több-a-többhöz)
* Kulcsmezők (elsődleges és idegen kulcsok) kijelölése
* Fontosabb mezők és adattípusok meghatározása

**5. Adatbázis létrehozása MS SQL Serverben**
* MS SQL Server Management Studio használata
* Adatbázis és táblák létrehozása SQL parancsokkal (CREATE DATABASE, CREATE TABLE)
* Kapcsolatok létrehozása (ALTER TABLE, ADD FOREIGN KEY)
* Néhány mintaadat felvitele a táblákba (INSERT INTO)

**6. Entity Framework modell generálása** 
* Az Entity Framework Database-first megközelítésének előnyei és hátrányai
* Entity Framework telepítése a Visual Studio projektbe
* Adatbázis kapcsolat létrehozása a projektben
* Database-first modell generálása az adatbázis alapján
* A generált modell áttekintése és megértése

**7. Gyakorlati feladat**
* Egyszerű adatlekérdezések futtatása a generált modellen keresztül LINQ használatával
* Az adatok megjelenítése a konzolon (Console.WriteLine)

**8. Összefoglalás és következő hét** 
* Az első hét anyagának összefoglalása
* A következő héten a CRUD műveletek és az adatelérés kerülnek fókuszba
* A hallgatók bátorítása kérdések feltevésére és az anyag átismétlésére

**Megjegyzés:** Az egyes pontokra fordított idő rugalmasan alakítható a hallgatók előzetes tudása és a csoport dinamikája alapján. Fontos, hogy az elméleti ismereteket azonnal gyakorlati példákkal támasszuk alá, és a hallgatókat aktívan bevonjuk az adatbázis tervezés folyamatába. 


### 2. Hét: CRUD Műveletek és Adatelérés

* **Entity Framework alapjai:** 
    * DbContext használata.
    * Adatlekérési és mentési műveletek (LINQ).
* **CRUD műveletek implementálása:**
    * Új adatok létrehozása (Create).
    * Adatok olvasása és megjelenítése (Read).
    * Adatok módosítása (Update).
    * Adatok törlése (Delete).

### 3. Hét: Windows Forms Felhasználói Felület

* **Felhasználói felület tervezése:**
    * Űrlapok létrehozása a különböző funkciókhoz (ügyfélkezelés, termékkezelés, számlakészítés stb.).
    * Adatmegjelenítő vezérlők használata (DataGridView, ListBox stb.).
    * Beviteli mezők és gombok elhelyezése.
* **Eseménykezelés:** 
    * Gombokhoz és vezérlőkhöz eseménykezelők rendelése.
    * CRUD műveletek összekapcsolása a felhasználói felülettel.

### 4. Hét: Validáció és Hibakezelés

* **Bevitt adatok ellenőrzése:**
    * Kötelező mezők, adattípusok, formátumok stb.
    * Hibajelzések megjelenítése a felhasználónak.
* **Hibakezelés:**
    * Kivételkezelés (try-catch blokkok).
    * Adatbázis tranzakciók használata az adatintegritás biztosítására.
    * Felhasználóbarát hibaüzenetek megjelenítése.

### 5. Hét: Számlakészítés és További Funkciók

* **Számlakészítés logikájának implementálása:**
    * Számla tételek hozzáadása és kezelése.
    * Számla végösszeg számítása.
    * Számla PDF generálása vagy nyomtatása.
* **További funkciók implementálása (opcionális):**
    * Keresés és szűrés.
    * Statisztikák és jelentések készítése.
    * Felhasználói jogosultságok kezelése.

### 6. Hét: Projektbemutató és Értékelés

* **Hallgatók bemutatják elkészült projektjeiket.**
* **Oktató értékeli a projekteket és a hallgatók egyéni teljesítményét.**
* **Reflexió a tanultakra és a projekt során szerzett tapasztalatokra.**

**Fontos:** A fenti terv rugalmasan kezelendő, az egyes témákra szánt idő a hallgatók előzetes tudásától és a projekt összetettségétől függően változhat. A hangsúly a gyakorlati munkán és az önálló problémamegoldáson van. Az oktató folyamatos támogatást és visszajelzést nyújt a hallgatóknak a projekt során. 
