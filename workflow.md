# Workflow für neue Pflanzen und Fusionen

Dieser Workflow gilt ab Plant Fusion 19.0.0 mit Plant Library 1.0.0. `worklfow.md` bleibt eine Verknüpfung auf diese Datei. Allgemeine API und Beispiel stehen in [Plant Library](../plant-library/README.md) und [example-mod](../plant-library/example-mod/README.md). Vertiefende native Belege und frühere Arbeitsweisen stehen in [workflow-referenz.md](workflow-referenz.md).

## Auftragsgrenzen

Seit Veröffentlichung der Debug-Mod-API sind Spielstart und automatisierte Laufzeittests für Implementierungs- und Prüfaufträge erlaubt. Starte das Spiel bei Bedarf über Steam; verwende die dokumentierte API in `../debug-mod/src/documentation.md` für temporäre Debug-Sitzungen, Zustandsprüfungen und Screenshots. Eine zusätzliche Startfreigabe ist dafür nicht erforderlich. Starte Steam als angemeldeter Desktopbenutzer (`kevin`), niemals als root. Aus einer Root-Sitzung verwende `rtk proxy runuser -u kevin -- env DISPLAY=:0 XDG_RUNTIME_DIR=/run/user/1000 DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus steam -applaunch 3654560`. Prüfe anschließend das API-Lebenszeichen und verwende bei Bedarf `startup.continue`; ein noch fehlendes Lebenszeichen ist kein Grund für einen zweiten Spielstart. Beende oder ersetze keine laufende normale Spielpartie. Recherche, Dateiprüfungen und Kompilierung belegen kein Laufzeitverhalten. Verändere keine nativen Profile oder Spielstände. Bibliotheksfreischaltungen werden erst während einer tatsächlichen Spielaktion in einer eigenen Datei gespeichert.

Nutze vorhandene Python- und .NET-Umgebungen. Alle Shellbefehle erhalten gemäß `/root/.codex/RTK.md` das Präfix `rtk`. Vor Codesuche zuerst den gemeinsamen CodeGraph-Index unter `/home/kevin/modding` verwenden. Nach Änderungen den Index aktualisieren. Subagenten sind nicht Bestandteil dieses Workflows.

Bei reinen Strategieaufträgen bleiben Quellcode, Assets, Versionen und installierte DLLs unverändert. Ein ausdrücklich beauftragter Pflanzenport umfasst dagegen alle belegten Fähigkeiten, Speicherung und Lebensdauer, Originalgrafiken und Animationen, vollständige englische und deutsche Almanachtexte, verfügbare alternative Rezepte, Dokumentation und sicheren Release.

## 1. Vertrag und vorhandene Besitzer prüfen

Verfolge vor Änderungen die konkrete Fähigkeit vom Auslöser bis zum tatsächlichen Zustands- oder Gesundheitsschreiber. Kläre Zielauswahl, Trefferreihenfolge, Abbruchbedingungen, Speichern/Laden, Pause, Simulationsticks, Tod und Pool-Wiederverwendung. Nutze bestehende Belege nur, wenn Quellenfassung und relevante Eingaben weiterhin passen.

Die Zuständigkeiten sind verbindlich:

- **Plant Library:** Pflanzenregistrierung, eindeutige Kennungen, reguläre Auswahl einschließlich Überlaufseiten, profilgebundene explizite Freischaltungen, automatische Mod-Tabs im Almanach, gemeinsame Projektil-, Effekt- und Soundregistrierung.
- **Plant Fusion:** Rezeptpaare, Zutatenbedingungen, Fusionstransaktionen, Zutatenverbrauch, besondere Terrain- und Unterfeldregeln, Fusionsfähigkeiten, Pflanzenverhalten, importierte Originalmodelle und deren Lebensdauer.
- **Native Systeme:** Board, Simulation, Terrain, Plant-/Projectile-Pools, Saatbank, Standardprojektile, Foley-Sounds, UI-Datenmodelle, Schriftarten und Eingabe.

Fusionieren bleibt vollständig in Plant Fusion. Auch ein zusätzlicher Almanach-Zusammenhang ist nur ein von Plant Fusion gelieferter Callback; die Bibliothek darf daraus keine Rezepte oder Fusionen ausführen.

Prüfe Zutaten mit `tools/ingredient-availability.json`: `available`, genaue `identity` und `evidence`. Ein Rezept oder Modell genügt nicht als Verfügbarkeitsbeleg. Entferne erst vollständig implementierte Rezepte aus `fusionen.txt`; gesperrte Alternativen bleiben offen. `fusionen-quellen.json` bleibt unverändert.

## 2. Identität und Registrierung anlegen

`tools/registrations.py` gibt einen Patch aus und schreibt keine Dateien. Wende den ausgegebenen Patch im Projektverzeichnis mit `patch -p1` an; kontrolliere danach mit `--check`. Das vermeidet einen Build mit noch nicht übernommenen generierten Identitäten.

Neue Fusion-Identitäten und Familien kommen in `tools/plant-registrations.json`. Generiere `FusionPlant.cs` und die markierte Rendererregistrierung über `tools/registrations.py`; bearbeite generierte Abschnitte nicht unabhängig. Bereits veröffentlichte native Typ-/Markerpaare bleiben unverändert. Quellspiel-IDs werden niemals als native `SeedType`- oder Projektil-Enums verwendet.

`FusionPlant.From` erhält die Identität bereits registrierter Pflanzen anderer Mods. Unbekannte Bibliotheksvarianten dürfen nicht versehentlich als unveränderte Vanilla-Zutaten behandelt werden. Nativer Imitater behält seinen bisherigen Weg.

Ergänze die Pflanze im bestehenden Fusion-Almanachkatalog mit stabiler ID, beiden Sprachen und Originalporträt. `FusionAlmanac.RegisterLibrary` übersetzt den Katalog beim Modstart in `Plants.RegisterPlant`-Aufrufe. Deshalb erhalten neue Katalogpflanzen automatisch ihren Bibliothekseintrag. `Plants.RegisterCatalog` veröffentlicht den Mod-Katalog einmal. Zusätzliche Variantenpaare oder IDs dürfen nicht mit anderen Mods kollidieren.

Neue eigenständige Mods verwenden dasselbe API direkt; die Beispiel-Mod zeigt den vollständigen Minimalweg. Initialisierung und Fähigkeitsticks lassen sich über `PlantDefinition.Initialize` und `Tick` ergänzen. Für native Schüsse steht `ProjectileCreated` zur Verfügung. Komplexe bestehende Fähigkeiten dürfen ihre bereits verantwortlichen Harmony-Patches und Renderer behalten; die Bibliothek ist kein Ersatz für konkrete Pflanzenlogik.

## 3. Freischaltungen festlegen

Zusätzliche Fusion-Varianten erhalten `UnlockPolicy.Never`. Normale Levelbelohnungen, Kaufwege, Laden und direkte Debug-Erzeugung schalten sie nicht frei. Vanilla-Pflanzen, die auch Fusionsergebnisse sind, behalten ihre normalen nativen Freischaltungen.

Einzige Ausnahme ist die Blutsonnenblume: `UnlockPolicy.Explicit`. `FusionService.CompleteFusion` ruft nach erfolgreicher Erzeugung `Plants.Unlock` für das aktive Profil auf. Auch eine tatsächlich abgeschlossene Fusion im Debug-Level zählt. Infektion, Laden und direktes Spawnen zählen nicht. Neue Varianten erhalten keine weitere Ausnahme ohne entsprechenden Auftrag.

Der Debug-Katalog darf gesperrte Varianten weiterhin anbieten. Die normale Bibliotheksauswahl verwendet ausschließlich `Always` oder tatsächlich freigeschaltete `Explicit`-Pflanzen. Eine neue Freischaltung erscheint beim nächsten Aufbau der Auswahl. Das Sidecar `UserData/PlantLibrary/unlocks.json` darf native Profile nicht ersetzen. Fehler beim Speichern dürfen eine bereits abgeschlossene Fusion nicht unterbrechen; beschädigte Dateien bleiben erhalten.

## 4. Fusion und Fähigkeit implementieren

Erweitere `FusionRecipes`, `AcceptsIngredient`, `FusionService` und die passende vorhandene Pflanzenfamilie. Erzeuge alle Ergebnisse vor dem Verbrauch der Zutaten. Bei fehlgeschlagener Erzeugung bleiben Zutaten erhalten. Stelle temporäre Kontexte in Fehlerpfaden wieder her. Erhalte Unterlagen, Kürbisse, Mehrfach-Unterfelder, Handschuhkosten und die bestehende Spielerzuordnung.

Native Felder speichern den laufenden Zustand. Flüchtige Wörterbücher benötigen Board- und vollständige DataID-Zuordnung sowie Bereinigung bei Boardende. Poolzeiger allein sind keine Identität. Schadens-, Todes- und Effektcallbacks können Objekte wiederverwenden; erfasse benötigte Daten vorher und prüfe danach die Generation. Initialisierung neuer Pflanzen ist kein Ladecallback.

## 5. Projektile, Aufpralleffekte und Sounds registrieren

Registriere gemeinsame Sounds mit `Projectiles.RegisterSound` und visuelle Aufpralleffekte mit `RegisterImpactEffect`. Nutze native Foley-Sounds und Partikel, wenn sie passen. Anschließend registriert `RegisterProjectile` Erkennungsfunktion und optionale Verknüpfungen. Verwende Mod-Präfixe und stabile Kennungen. Erkennungsfunktionen dürfen sich nicht überlappen.

Die Erkennung muss aus gespeicherten Feldern rekonstruierbar sein. Für ein natives Projektil einer registrierten Pflanze setzt `ProjectileCreated` bei Bedarf dessen Kennung. Ein eigener Skript-Erzeuger setzt seine Kennung und ruft danach `NotifyBorn` auf. Geburtseffekte dürfen weder bei Grafikwechsel noch mehrfach pro Poolgeneration ausgelöst werden.

`ProjectileImpact.Capture` hält Board, Position und Zeichenreihenfolge vor der Kollision fest. Visuelle Callbacks lesen nach Schaden oder Tod keinen möglicherweise wiederverwendeten Projektilzustand. Bestehende besondere Kollisionsroutinen verwenden `EmitImpactEffect` mit ihrem zuvor erfassten Snapshot. Schaden, Statusübertragung und Kollisionsregeln bleiben in der Mod. Bei eigenem Ersatz eines nativen Aufpralls verhindert `ImpactOnlyWhenOriginalSkipped` doppelte Sounds. Bereits vorhandene Aufpralleffekte nicht nochmals über den automatischen Weg auslösen.

## 6. Originalgrafik und UI anbinden

Ergänze benötigte Prefab-IDs im vorhandenen Exporter und exportiere neue beziehungsweise geänderte Modelle gemeinsam über wiederholtes `--plant`, nicht unnötig über `--all`. Erhalte Originalanimationen, Ebenen, Mischmodi, Größe, Anker, Bindungen und Aktionsclips. Genau ein System besitzt Bewegung. Animationsevents erzeugen keine doppelten Schüsse. Blende native Grafik erst nach erfolgreichem Import aus.

Verwende native UI-Datenmodelle, Karten, Buttons, Schriftarten, ScrollRect und Navigation. Reguläre Auswahlseiten und Almanach-Tabs werden nicht nochmals in Plant Fusion implementiert. Erweiterungen dieser Ansichten gehören in Plant Library. Der eigene Debug-Level bleibt eine getrennte Fusion-Funktion und meldet seine Boards über `Plants.ExternalChooser` an.

Jede Pflanze benötigt vollständige englische und deutsche Texte, Originalporträt und belegte Anpassungen. Prüfe nach Änderungen englischer Texte den deutschen Übersetzungsbezug `EnglishSha256`. Sprache folgt der nativen Spielsprache. Nicht portierte Zutaten und Synergien bleiben ausdrücklich abgegrenzt.

## 7. Plant Library bei tatsächlichem Bedarf erweitern

Prüfe zuerst, ob bestehendes API, native Plattform oder vorhandener Besitzer die Fähigkeit bereits abdeckt. Fehlt eine allgemeine Funktion für den konkreten Port, ergänze sie am zuständigen Besitzer in `../plant-library/src/`. Dokumentiere Verhalten, Lebensdauer, Fehlerfälle und einen konkreten Aufrufer. Ergänze das Beispiel, wenn es den neuen Weg sinnvoll demonstriert.

Kopiere keine allgemeinen Registrierungen, Auswahlseiten oder Almanach-Ansichten zurück nach Fusion. Verschiebe umgekehrt keine Fusion-Rezepte oder konkrete Pflanzenfähigkeiten in die Bibliothek. Bewahre stabile öffentliche Signaturen und Speicherkennungen. Falls eine Änderung bestehende Aufrufer bricht, passe alle betroffenen Mods im selben Auftrag an und erhöhe die Bibliotheksversion entsprechend. Keine Schnittstellen für lediglich vermutete spätere Funktionen anlegen.

## 8. Dokumentation und kleinster ausreichender Nachweis

Aktualisiere Pflanzen-Portnotizen, Katalog, README und relevante gemeinsame Dokumentation. Beschreibe gemeinsame Mechanik und Belege einmal; verlinke sie aus den Portnotizen. Die Quellenbelege müssen tatsächliche Fähigkeiten und Grenzen erklären und dürfen nicht nur aus automatisch erzeugten Rezeptlisten bestehen.

Kompiliere gegen die installierten Referenzen. Bei Änderungen am gemeinsamen API zusätzlich die Beispiel-Mod kompilieren:

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/fusion-mod/FusionMod.csproj -c Release
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/plant-library/example-mod/PlantLibraryExample.csproj -c Release
rtk proxy python3 /home/kevin/modding/pvzreplanted/fusion-mod/tools/registrations.py --check
```

Nach neuen Almanacheinträgen `tools/almanac.py` ohne Unterbefehl ausführen und anschließend `tools/build_debug_cards.py` ausführen. Für beide Skripte `/home/kevin/modding/pvzreplanted/.tools/fusion-workflow/bin/python` verwenden; das System-Python enthält nicht die benötigten Exportabhängigkeiten. Damit werden Katalog und Debug-Karten in einem Durchgang vollständig aktualisiert. Beide Skripte exportieren nur Dateien und starten weder Spiel noch Tests.

Für wiederholte API-Tests mit Debug Mod 1.3.0 starte Steam mit `--debug-level=Day` (alternativ `Night`, `Pool`, `Fog`, `Roof` oder einem Hintergrundindex). Der Direktstart bestätigt Startbildschirm und beide Kartenwahlen mit Platzhaltern; dadurch entfallen die wiederholten API-Aufrufe für Hintergrundwahl und Rosterbestätigung. Warte vor Testmutationen auf `status.debug: true` und `status.scene: Playing`. Ersetze danach bei Bedarf Platzhalter über `bank.set` und `debug.roster`. Beende eigene Testprozesse mit `game.shutdown` und warte anschließend auf das tatsächliche Prozessende. Unter Proton kann das Beenden mehrere Sekunden dauern; Steam ignoriert einen währenddessen angeforderten Neustart. Verwende keinen zweiten Start bei noch laufendem Ladevorgang.

Bei installierter ReplantAPI kann der Direktstart bereits ein spielbares Board melden, während dessen Startbild die Spielansicht noch überdeckt. Prüfe deshalb vor Grafiktests einen Screenshot. In diesem nachgewiesenen Fall spart ein normaler Steam-Start ohne `--debug-level`, gefolgt von `startup.continue`, dem fertigen Hauptmenü und `debug.start`, unbrauchbare Grafik-Testläufe. Zustandsprüfungen allein belegen weder sichtbare importierte Körper noch sichtbare Enten; prüfe zusätzlich deren aktive Renderer und den Screenshot.

Bei `plant.place` bestimmt `offsetY` die vertikale Position im Feld (Standard 40). Verwende `offsetY:70` für Blumentopf-Fusionen im unteren Felddrittel; damit entfällt das manuelle Positionieren der Maus und die API-Prüfung bleibt reproduzierbar.

Führe bei Implementierungsaufträgen die kleinsten ausreichenden Offline- und API-Laufzeittests selbst aus. Prüfe neue Fähigkeiten einschließlich Schaden, Zielauswahl, Projektilaufprall, Düngung und betroffener UI. Starte das Spiel bei Bedarf gemäß den Auftragsgrenzen. Warte bei der Kartenwahl auf `chooser.ready: true`, nicht nur auf `visible`; das native Panel kann sichtbar sein, bevor seine Bindungen bereit sind. Prüfe nach Bestätigung den tatsächlichen Phasenwechsel. Räume eigene temporäre Debug-Sitzungen auch nach fehlgeschlagenen Tests auf. Bewahre JSON-Ergebnisse und relevante Screenshots als Nachweis auf. Behaupte keine bestandenen Spieltests aufgrund von Build, Export, Testdoubles oder Dateiansichten. Nenne im Abschluss konkrete verbleibende Laufzeitprüfungen, soweit sie vom Auftrag betroffen sind.

## 9. Version und gemeinsamer Release

Fusion verwendet `Gameplay.NeuePflanzenOderZombies.Fixes`. Lies die aktuelle Version aus `FusionMod.csproj`. Änderungen bestehender Spielregeln erhöhen die erste Stelle, neue Pflanzen oder Zombies die zweite, Fehlerkorrekturen die dritte. Setze nachfolgende Stellen auf null. Höchste betroffene Klasse bestimmt einen Versionswechsel pro Auftrag. Dokumentation allein erhöht keine Modversion. Halte Projektversion, `MelonInfo` und README konsistent. Die Bibliothek besitzt eine eigene API-Version.

Baue und installiere Bibliothek und Fusion gemeinsam:

```sh
rtk proxy python3 /home/kevin/modding/pvzreplanted/plant-library/tools/install.py
```

`--example` installiert zusätzlich die Demonstrationspflanze; nur bei entsprechendem Auftrag verwenden. Ohne diese Option bleibt sie Quellcode und Build-Artefakt im Unterordner.

Der Installer verwendet die gemeinsame Releasesperre, sichert vorhandene DLLs, prüft unveränderte Installationsdateien und ersetzt sie atomar. Er schreibt Prüfsummen und einen Installationsnachweis. Bei Fehlern während der DLL-Ersetzung stellt er die vorherigen Dateien wieder her. Er startet weder Spiel noch Tests. Alte Einzeldatei-Kopierbefehle sind für die neue Abhängigkeit unvollständig.

Für den Rückbau Spiel schließen und das zusammengehörige DLL-Paar aus dem dokumentierten Backup wiederherstellen. Neu hinzugekommene Dateien gemäß Installationsnachweis entfernen. Keine nativen Profile oder Saves löschen. Danach den gemeinsamen CodeGraph-Index aktualisieren:

```sh
rtk proxy codegraph sync /home/kevin/modding
```
