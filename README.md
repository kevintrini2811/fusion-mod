# Fusion-Mod 22.1.0

## Vier neue Fackelfusionen (22.1.0)

Nussholz, Dreifachfackel, Funkelholz und Hypnofeuer verwenden Originalmodelle, Animationen und Projektilgrafiken aus Fusion 4.0.5.2. Alle vier Rezepte funktionieren in beiden Reihenfolgen. Dünger heilt und erhält die gespeicherten Entzündungszähler. [Quellen und Umsetzung](references/torch-four-22-1-0/implementation.md).

## Projektilkonversionen korrigiert (22.0.1)

Scorchwood taut Schneeerbsen einschließlich ihrer Grafik nativ auf und verwandelt gelbe Feuererbsen sowie Kartoffelerbsen korrekt. Die Quelle verlangt orange Feuererbsen; das zuvor verwendete violette Modell ist ersetzt. Kartoffelerbsen wechseln beim Überqueren aus ihrer eigenen Bewegung in die native Bewegung. Nicht unterstützte Blaze-Erbsen bleiben bei beiden Feuerholz-Fusionen unverändert. [Testmatrix mit Vorher-/Nachhergrafik und Treffern](references/torch-projectiles-22-0-1/notes.md).

## Neue Fusionen und Wasserregeln in 22.0.0

Tide-shroom, Blacklight Plantern, Scorchwood und Stenchwood sind mit Originalgrafiken, verfügbaren Rezepten und Düngerregeln ergänzt. Neun weitere native Zombies erhalten Entenvarianten. Ballons überleben Wasser und landen nach dem Platzen; Pogo verliert seinen Stab beim Wassereintritt. Wegen der geänderten Wasserregeln steigt die erste Versionsstelle. Debug Mod 1.3.2 verhindert dauerhafte Almanach-Sichtungen aus Debug-Leveln. [Portvertrag, Quellen und Prüfbelege](references/support-ducks-22-0-0/notes.md).

## Neue Fusionen in 21.5.0

Schirmkohl (Kohlapult + Schirmblatt), Schirmrinde (Melonapult + Schirmblatt), Lichtschirm (Plantern + Schirmblatt) und Eistopf (Blumentopf + Eispilz) verwenden Originalmodelle, Animationen und beide Rezeptreihenfolgen. Der Eistopf enthält die vier originalen, nicht fusionierbaren gerösteten Hilfsformen. Gletscherprojektile behalten ihre Originalgrafik. Debug Mod 1.3.1 ergänzt Kartenrahmen sowie Controller-Blättern an den horizontalen Kartenrändern. [Portvertrag und Prüfbelege](references/umbrella-ice-21-5-0/notes.md).

## Neue Fusionen in 21.4.0

Threetato Mine (Potato Mine + Threepeater) und Marigold Mine (Potato Mine + Marigold) sind mit Originalgrafiken, Animationen, Projektillogik, Düngung und beiden Rezeptreihenfolgen portiert. Die zweite Versionsstelle steigt wegen zweier neuer Pflanzen. [Originalbelege, Portvertrag und Spieltests](references/treasure-three-mines-21-4-0/port-notes.md).

Debug Mod 1.3.0 ergänzt Direktstart mit `--debug-level=Day`, Spielende, Difficulty- und Sun-Collector-Steuerung, goldene Kaffeebohne, Modusfreischaltungen, vollständige Statusabfragen, Profilverwaltung, Grabsteine, Rasenmäher und virtuelle Xbox-360-Eingabe. Die [API-Dokumentation](../debug-mod/src/documentation.md) beschreibt Befehle und Sicherheitsgrenzen.

## Neue Fusionen in 21.3.0

Chompkin, Pumpkin Mine, Sunny Pumpkin und Glacier-shroom sind mit Originalgrafiken, Animationen und allen sechs verfügbaren Rezepten portiert. Die zweite Versionsstelle steigt wegen der vier neuen Pflanzen. Dünger berücksichtigt die jeweilige Fähigkeit. [Portnotizen und API-Nachweise](references/pumpkin-glacier-21-3-0/port-notes.md).

## Cherry Pop und Ball Cactus (21.1.0)

Zwei neue Fusionen mit nativ verfügbaren Zutaten: Puff-shroom + Cherry Bomb ergibt Cherry Pop; Puff-shroom + Cactus ergibt Ball Cactus. Beide Rezeptreihenfolgen verwenden die bestehende Fusionstransaktion. Es gibt in Quelle 4.0.5.2 keine weiteren Rezepte für diese Ergebnisse.

Cherry Pop explodiert bei Bodenkontakt oder Tod mit 900 Schaden und befestigt zuvor eine halbreife Kirsche an einem zufälligen geeigneten Ziel ohne Haftkirsche. Die erste Explosion lädt diese Kirsche bereits auf. Ball Cactus schießt alle etwa eine Sekunde einen Originalkaktusstachel mit 30 Schaden über die ganze Reihe. Er verkleinert nach dem Treffer sowohl Boden- als auch Flugziele entsprechend dem tatsächlichen Quellverhalten. Wichtel verwenden die ursprüngliche Todesausnahme. Originalmodelle, Animationen, Haftkirschen, Aufpralleffekte, Tagesruhe und drei Unterfelder sind angebunden. Düngung heilt ohne zusätzliche Entwicklung.

[Quellen und Nachweise](references/puff-duo-21-1-0/port-notes.md). Debug Mod 1.2.1 verwendet denselben grauen Roster wie die Zombieauswahl und einheitliche Saatpakete für registrierte Zombies.

## Puff-shroom-Ausbreitungen (21.0.0)

`Puff-shroom (Unterlage) + Fume-shroom` verbraucht ein bis drei gewöhnliche Puff-shrooms derselben Zelle. Bei einem, zwei oder drei Zutaten entstehen vorwärts `1`, `2/1` beziehungsweise `3/2/1` Puff-shrooms. In der Ursprungszelle bleibt ein nativer Fume-shroom.

`Puff-shroom (Unterlage) + Scaredy-shroom` erzeugt bei einer Zutat einen Puff-shroom vorwärts. Bei zwei Zutaten entsteht je einer vorwärts, oben und unten; bei drei Zutaten jeweils zwei. In der Ursprungszelle bleibt ein nativer Scaredy-shroom. Gesperrte Zellen werden übersprungen; freie Unterfelder bleiben nutzbar. Gemischte Pilzgruppen bleiben geschützt. Karten und Handschuh verwenden denselben Transaktionsweg.

Beide Rezepte enden in vorhandenen Replanted-Pflanzen. Fähigkeiten, Grafiken, Projektile, Aufpralleffekte, Tagesruhe, Düngung und Almanacheinträge bleiben nativ. Keine zusätzlichen Pflanzentypen oder Freischaltungen. Die erste Versionsstelle steigt wegen der neuen Ausbreitungsmechanik. [Quellen, Prüfungen und Grenzen](references/spreading-21-0-0/port-notes.md).

Die getrennte [Debug Mod 1.2.0](../debug-mod/README.md) zeigt während der Zombieauswahl graue UI-Rahmen und Hirne statt Sonnen. Nach Auswahl oder Abbruch werden die nativen Grafiken wiederhergestellt.

## Debug Mod ausgelagert (20.1.0)

Der Debug-Levelbetrieb liegt jetzt in [Debug Mod 1.0.0](../debug-mod/README.md). Ohne `DebugMod.dll` startet Fusion keinen Debug-Level. Mit Debug Mod bleiben Fusion-Zutaten, fertige Fusionen, Platzierungsregeln und Vorschauen verfügbar. Die Debug-Auswahl übernimmt zusätzlich alle registrierten Einträge anderer Mods aus Plant Library und Zombie Library, einschließlich normalerweise gesperrter Varianten. Die sieben eigenen Mods und Debug Mod deklarieren einander als optionale Abhängigkeiten; bestehende Abhängigkeiten zwischen den anderen Mods bleiben erhalten. Dieses Release enthält Plant Library 1.0.3 und Zombie Library 1.1.3 für die Kataloganbindung. Spiel und Testprogramme wurden nicht gestartet.



## Hypnokopfrichtung und Debug-Almanach (20.0.4)

Der animierte Kopf des Hypnoerbsen-Zombies wird im nativen Kopfanker horizontal gespiegelt. Seine Position berücksichtigt den gespiegelten Mittelpunkt; native Richtungswechsel bleiben zuständig. Der Screenshot `Bildschirmfoto_20261103_121240.png` zeigt die vorherige falsche Richtung entgegen dem nach rechts fliegenden Projektil.

Die vollständige Zombiebeute-Sperre im Debugmodus ist auf Benutzerwunsch wieder aktiv. Sie überspringt `Zombie.DropLoot` und setzt `mDroppedLoot`; Münzen und native Levelbelohnungen bleiben dort aus. Normale Level behalten ihre Beute.

Zombie Library 1.1.2 berücksichtigt die temporäre Debug-Freigabe bei nativen Zombieeinträgen, Zombotany und Mod-Tabs einschließlich Entenvarianten. Außerhalb des Debugmodus gelten wieder die tatsächlichen Sichtungen. Dafür werden keine künstlichen Sichtungen oder nativen Profilfreischaltungen gespeichert. [Details und manuelle Abnahme](references/feedback-20-0-4/report.md).

Spiel und Testprogramme wurden nicht gestartet. Die Laufzeitprüfung übernimmt der Benutzer.


## Grafik, Flugziele und Zombiebeute (20.0.3)

Gatling-Fusionen mit nativem Gatling-Träger gleichen ihre vollständige Höhe an die tatsächliche native Meshpose an. Die statische Vorschaugrafik und die Schattenbreite sind dafür keine zuverlässigen Maße. Ungültige Spine-Koordinaten bleiben ausgeschlossen. Butter Fly und Cabbage Dropper verwenden die vorhandene Umrechnung der Quellmodelle statt einer Vergrößerung ihres schmalen Schattens auf Kachelbreite; beide werden dadurch rund 35 Prozent kleiner.

`Zombie.CanTargetPlant` schließt Pflanzen der Flugschicht bereits bei der Kandidatenauswahl aus. Zombies laufen daran vorbei und können weiterhin Bodenpflanzen darunter fressen. Die vollständige Debug-Sperre auf `Zombie.DropLoot` entfällt: Native Beute einschließlich Münzen läuft wieder, mit unveränderten Chancen und Profilgutschrift. Nur native Levelbelohnungen bleiben im Debugmodus gesperrt.

Der Hypnoerbsen-Zombie erhält in Almanach, Karten und Entenkarten ausschließlich den Kopf einschließlich Mundstück. Der Export entfernt Stängel, Blätter und Schatten. Auch die animierte Kopfdarstellung berücksichtigt das separat angelegte Mundstück. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert.

[Ursachen, Nachweise und manuelle Prüfpunkte](references/feedback-20-0-3/report.md). Spiel und Testprogramme wurden nicht gestartet. Kompilierung und Dateiprüfungen ersetzen keine Spieltests; die Abnahme übernimmt der Benutzer.

## Geschwindigkeitssteuerung ausgelagert (20.0.2)

Schnellvorlauf und Zeitlupe gehören jetzt zur eigenständigen [Speed Mod](../speed-mod/README.md). Fusion benötigt `SpeedMod.dll`, verwendet deren UI-Anker und übernimmt die Begrenzung sichtbarer importierter Animationen auf 2× ab Spieltempo 10×. Importierte Projektile überspringen dann Posenberechnung und visuelle Drehungen. Simulation, Treffer und Projektilflug bleiben unverändert. Build und statische native Analyse ersetzen keinen Spieltest; die manuelle Abnahme übernimmt der Benutzer.

## Debug-Levelübergang (20.0.1)

Nach der Zombieauswahl setzt die native `SeedBank.UpdateWidth` die Kartenzahl erneut aus `Board.GetNumSeedsInBank`. Der bisherige Debug-Patch lieferte weiterhin zehn, selbst wenn nur eine Sonnenblume gewählt war. Die zuvor gesetzte tatsächliche Kartenzahl wurde dadurch überschrieben. Der Debug-Chooser speichert jetzt die bestätigte Pflanzenzahl vor dem nativen Abschluss. Während der Auswahl bleiben zehn Plätze verfügbar; im Level liefert derselbe Kapazitätsbesitzer die tatsächlich gewählte Zahl. Ein erneuter Breitenaufbau kann die Bank damit nicht mehr mit geleerten Karten auffüllen.

Die goldene Kaffeebohne verwendet weiterhin den Slowdown-Container als Positionsanker. Ihre Button-Vorlage stammt wieder vom tatsächlichen nativen Speed-Button: Der Container selbst besitzt keine `Button`-Komponente. Diese Regression aus 20.0.0 konnte den HUD-Aufbau abbrechen.

Plant Library und Zombie Library wurden einschließlich ihrer Auswahl- und Bankpatches geprüft und gemeinsam mit Fusion neu kompiliert. Es war keine Änderung ihrer API oder ihres Quellcodes erforderlich. Native Profile und Spielstände bleiben unverändert. [Gesicherte Logs, native Analyse und Prüfgrenze](references/debug-crash-20-0-1/report.md).

Die Logs enthalten keinen fatalen Stacktrace. Der genaue native Absturzpunkt und Crashfreiheit sind deshalb nicht bewiesen. Spiel und Testprogramme wurden nicht gestartet. Der Benutzer prüft erneut den Übergang mit einer Sonnenblume und einem normalen Zombie, anschließend mehrere Karten und den Kaffeebohnen-Button.

## Änderungen 20.0.0

- Neue Fusionen: **Blover + Cabbage-pult = Cabbage Dropper** und **Blover + Kernel-pult = Butter Fly**, jeweils auch umgekehrt. Vollständige Originalmodelle, Animationen, drei ballistische Abwürfe, Butterwirkung, Aufpralleffekte und Almanachtexte sind eingebunden. [Portbelege und manuelle Prüfung](references/air-lobbers-20-0-0.md).
- Die goldene Kaffeebohne steht über dem Slowdown-Knopf. Der Levelmünzzähler steht links neben der Kaffeebohne.
- Pflanzenmünzen gehen ausschließlich in den Levelvorrat; Zombiebeute und sämtliche Münzen im Zen Garden in den Profilvorrat. Herkunft und besondere Auszahlungswerte bleiben bei neuen Münzen über Speichern/Laden erhalten. Bereits vor diesem Update erzeugte unmarkierte Münzen behalten den Profilweg. [Verhalten und native Belege](references/currency-20-0-0/port-notes.md).
- Mit installierter Difficulty-Mod ersetzt der Augenzombie auf **Extrem 33 %** der geeigneten normalen Zombies. Sonst bleiben es **1 %**. Difficulty-Mod bleibt optional; Fusion enthält keine Assembly-Abhängigkeit darauf.
- Das Mod-Menü verwendet Chompzilla auf hellem nativem Rasen. Der Hypnoerbsen-Zombie besitzt auch im Zombie-Almanach und dessen Karten den importierten Hypnoerbsenkopf.

Die erste Versionsstelle steigt wegen der geänderten Münz- und Extrem-Spielregeln. Beide neuen Fusionen sind aus `fusionen.txt` entfernt. Spiel und Testprogramme wurden nicht gestartet. Kompilierung und statische Prüfungen ersetzen keine Spieltests; diese übernimmt der Benutzer.

## Native Zombie-Karten (19.1.1)

Zombie Library 1.0.1 ersetzt graue Kartenflächen durch native Zombie-Rahmen und originale Almanach-Nahansichten. Die zusätzlichen grünen Namen unter Karten entfallen. Namen und Beschreibungen bleiben in der nativen Detailansicht. Eigene Vorschaubilder unterscheiden normale und Gummientenvarianten. Hover und Controller-Auswahl verwenden die native Markierung. Die Änderungen beruhen auf den Screenshots `Bildschirmfoto_20261103_070315.png` und `Bildschirmfoto_20261103_070325.png`.

Spiel und Tests werden weiterhin ausschließlich vom Benutzer ausgeführt.

## Zombie Library (19.1.0)

Fusion benötigt jetzt zusätzlich `ZombieLibrary.dll`. Hypnoerbsen-Zombie und Augenzombie registrieren ihre bestehenden gespeicherten Identitäten in dieser Bibliothek. Initialisierung und Beschwörung laufen über die Library; Spezialverhalten bleibt in Fusion. Der Aufwachlaut des Augenzombies verwendet die gemeinsame Soundregistrierung.

Die Library übernimmt die bisherige native Pooldarstellung einschließlich Wasserlinien-Clipping, Armreihenfolge und Tänzer-Aufstehphase. Beide Spezialzombies erhalten Gummientenvarianten. Dazu kommen die acht bisherigen zusätzlichen nativen Pooltypen als eigene Varianten im Zombie-Almanach. Der Tab „Plant Fusion“ enthält insgesamt zwölf Einträge: zwei Spezialzombies und zehn Entenvarianten. Native Zombie-Karten, Hintergründe und Detailansichten werden verwendet.

Quellcode, API, optionales Beispiel und manuelle Prüfpunkte: [Zombie Library](../zombie-library/README.md). Der Library-Installer und der Fusion-Releasepfad installieren Plant Library, Zombie Library und Fusion gemeinsam mit DLL-Backups. Speichermarker bleiben unverändert. Spiel und Tests wurden nicht ausgeführt.

## Änderung 19.0.0

Plant Fusion benötigt jetzt `PlantLibrary.dll` zusätzlich zu `LawnGlove.dll`. Pflanzenregistrierung, normale Auswahlseiten, Mod-Tabs im Almanach sowie die gemeinsame Registrierung von Projektilen, Aufpralleffekten und Sounds liegen in der [Plant Library](../plant-library/README.md). Rezepte, Zutatenverbrauch, Terrainregeln und Fusionseffekte bleiben in Plant Fusion. Bestehende Pflanzen- und Projektilkennungen bleiben erhalten. Registrierte Pflanzen anderer Mods werden nicht als unveränderte Zutaten ihrer nativen Trägerpflanze behandelt.

Fusion-Varianten sind regulär gesperrt. Die Blutsonnenblume wird nach einer tatsächlich abgeschlossenen Fusion für das aktive Profil dauerhaft auswählbar. Direktes Debug-Spawnen, Infektion und Laden schalten sie nicht frei. Eine echte Fusion im Debug-Level zählt ebenfalls. Der Debug-Katalog bleibt vollständig. Neue Freischaltungen werden erst beim nächsten Öffnen der regulären Pflanzenauswahl angezeigt. Die Bibliothek speichert sie separat unter `UserData/PlantLibrary/unlocks.json`.

Bibliothek und Fusion gemeinsam mit `../plant-library/tools/install.py` bauen und installieren. Beispiel-Mod befindet sich unter `../plant-library/example-mod/`; ihre Installation ist optional. Spiel und Testprogramme wurden nicht gestartet. Kompilierung gegen die installierten Spielreferenzen ersetzt keine Laufzeitprüfung. [Aktueller Arbeitsablauf](workflow.md).

## Änderung 18.1.4

Fix für rote Fehler beim Fortsetzen eines Levels: Der Patch auf `Zombie.Deserialize(ref ReloadedBinaryReader, int)` entfällt. Damit läuft der native binäre Zombie-Reader ohne IL2CPP-Harmony-Trampolin. Flüchtige Gift-/Frostzustände werden stattdessen vor `SerializationService.Deserialize()` für das betroffene Board bereinigt. Gespeicherte Statuseffekte bleiben unverändert und werden wie bisher beim ersten `UpdateGame` wiederhergestellt; Überhitzung bleibt in ihren nativen GridItems gespeichert. Keine Spielstände werden gelöscht, umgeschrieben oder zurückgesetzt. Release-Build und statische Release-Prüfungen erfolgen ohne Spielstart. Spieltests und Testprogramme bleiben auf Nutzerwunsch unausgeführt.

## Änderung 18.1.3

Fixrelease für das zu große Hirn und den offenen linken Rand im Debug-Zombieroster. Das Hirn erhält eine begrenzte Fläche mit Abstand zum Counterzettel und zum oberen Rand. Die linke Hintergrundkante übernimmt gespiegelt den geschlossenen rechten Abschluss der nativen Textur. [Details](references/roster-18-1-3/report.md). Keine neuen Fusionen. Spielstart und Spieltests bleiben beim Benutzer.

## Änderung 18.1.2

Fixrelease für den Debug-Zombieroster: Kartenbreite, Kartenhöhe, horizontale Position und Abstand folgen den sichtbaren nativen Pflanzenkarten und ihrer Layoutgruppe. Die Position des Hirncounters folgt dem Sonnenlabel; die Rosterfläche schließt jetzt auch den Sonnenbereich ein. Der braune Hintergrund des Counterzettels wird beim Laden entfernt. Karten und Klickflächen verwenden dieselbe Geometrie. Keine neuen Pflanzen, Zombies oder Rezepte. Das Spiel und Testprogramme werden nicht gestartet; die visuelle Prüfung übernimmt der Benutzer. [Details](references/roster-18-1-2/report.md).

## Änderung 18.1.1

Fixrelease für die Rückmeldung vom 2. November: Der Debug-Zombieroster ersetzt den orangebraunen Farbton durch entsättigtes Graulila. Seine Fläche übernimmt Breite, Höhe und horizontale Position des tatsächlich sichtbaren Pflanzenrosters; Kartenabstände und Klicksperre folgen der neuen Geometrie. Die Ente des Augen-Zombies richtet ihre Größe nach dem sichtbaren importierten Torso statt nach der verdeckten nativen Entenpose. Im konzentrierten Schussmodus waren sechs Erbsen als zwei überlappende Dreiergruppen vorhanden. Jetzt folgen die Erbsen innerhalb beider Gruppen im Abstand von 0,08 Sekunden. Anzahl, Grundschaden und Angriffstakt bleiben erhalten. [Belege und Benutzerprüfung](references/feedback-18-1-1/report.md). Spiel und Testprogramme werden nicht gestartet.

## Änderung 18.1.0

Zwei neue Fusionen: **Mehrfacherbsenwerfer** (Peashooter + Threepeater) mit drei gespeicherten Schussmodi und **Meeresblumentopf** (Flower Pot + Sea-shroom) mit Wasserprüfung und nicht stapelbarem 50-Prozent-Geschwindigkeitsbonus für die verfügbaren Tanghybriden. Originalmodelle, alle Animationsclips, Düngerbehandlung und deutscher/englischer Almanach sind ergänzt. Beide abgeschlossenen Rezepte sind aus `fusionen.txt` entfernt.

Der Debug-Zombieroster verwendet den nativen Pflanzenrosterhintergrund mit lila Färbung, ein originales Hirnsprite und einen ∞-Counter. Kartennamen sind dunkel, passen sich der Kartenfläche an und erhalten bei Bedarf Auslassungspunkte. Der weiße Hinweistext entfällt. Bei ausgewählter Zombie- oder Pflanzenkarte hat der gesamte Zombieroster 20 Prozent Deckkraft und blockiert keine Klicks. Platzierung und Abbruch stellen die Anzeige wieder her. Auch die native Sonnenanzeige zeigt im Debugmodus ∞. [Belege und Benutzerprüfung](references/multipeater-sea-pot-18-1-0/report.md). Spiel und Testprogramme werden nicht gestartet.

## Änderung 18.0.6

Fixrelease für den abgeschnittenen Kopf des getragenen Imps bei Gargantuar und Giga-Gargantuar im Pool. Das native Schwimmrechteck reicht jetzt bei diesen beiden Typen eine vollständige Bildschirmhöhe über die Wasserlinie. Seine Unterkante bleibt unverändert; Körperbeschnitt, Enten- und Armreihenfolge sowie Imp-Wurf bleiben zuständig. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Ursache und Benutzerprüfung](references/imp-clip-18-0-6/report.md). Spiel und Testprogramme werden nicht gestartet.

## Änderung 18.0.5

Fixrelease für die Poolgrafik. Nur der linke Arm wird vor die Gummiente sortiert; der rechte Arm bleibt dahinter. Die übrigen Teilmeshes behalten ihre native Zeichenreihenfolge innerhalb einer gemeinsamen SortingGroup, damit insbesondere Football-Helm, Kopf und Gargantuar-Körper nicht durch gleiche Sortierwerte vertauscht werden. Die Entenspiegelung übernimmt das Vorzeichen der laufenden Pose auch beim Umdrehen des Tänzers. Die Yeti-Ente wird um 15 Prozent verkleinert. Beim Augen-Zombie liegt die Ente innerhalb seiner importierten Spritegruppe zwischen Körper und linkem Arm. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Ursachen und Benutzerprüfung](references/pool-layers-18-0-5/report.md). Spiel und Testprogramme werden nicht gestartet.

## Änderung 18.0.4

Fixrelease für verbleibende UI-, Flug- und Poolfehler. Geldbetrag der goldenen Bohne und Slowdown-Beschriftung übernehmen Darstellung und tatsächliche Schriftgröße der nativen Fast-forward-Anzeige. Flytrapper/Flug-Chomper verwendet dieselbe Flugschicht wie die anderen Flugpflanzen, einschließlich Karten-, Debugkarten- und Handschuhplatzierung. Spine-Armgrenzen beginnen jetzt am korrekten Slot und folgen Änderungen der Zeichenreihenfolge. Enten und importierte Sprites verwenden die tatsächliche native Schnittkante einschließlich Controller- und Bildschirmversatz statt einer Schattenabschätzung. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Belege und Benutzerprüfung](references/feedback-18-0-4/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 18.0.3

Fixrelease für die goldene Kaffeebohne, Slowdown, Flugpflanzen, Topfplatzierung mit dem Handschuh und Poolgrafik. Die Bohne erhält den nativen Knopfhintergrund und steht mittig über Fast-forward; der Slowdown-Text steht links und erscheint nur bei aktivem Slowdown. Die Flugschicht verwendet eine ausdrückliche Mod-Identität statt der nativen Kaffeebohnenklassifikation. Bodenpflanzen bleiben beim Platzieren erhalten. Der Handschuh darf normale Pflanzen auf leere native und fusionierte Töpfe setzen. Der Poolbeschnitt verwendet Rechteckpixel; synthetische Enten schwimmen an der Wasserlinie. Vorhandene Spine-Separatoren werden erweitert und ihre Armreihenfolge nach dem tatsächlichen Meshaufbau gesetzt. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Ursachen und Benutzerprüfung](references/feedback-18-0-3/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 18.0.2

Fixrelease für Flugpflanzen, Debug-Handschuh und Poolgrafik. Flugplatzierung verwendet die nativen Blover-Regeln auch für Angstdrohnen und berücksichtigt native Flugpflanzen beim Handschuh. Bodenpflanzen bleiben erhalten; Zutatenfusionen haben Vorrang. Der Debug-Handschuh lädt unmittelbar nach Benutzung wieder auf. Gargantuars, Giga-Gargantuars, Springteufel, Tänzer, Gruppentänzer und Football-Zombies erhalten den tatsächlichen Körperversatz ins Wasser. Der bisherige reine Beschnittversatz entfällt. Neue Gummienten behalten das originale Seitenverhältnis; native Armteile werden davor gerendert. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Belege und Benutzerprüfung](references/pool-air-18-0-2/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 18.0.1

Fixrelease für Debug-Almanach und Pool-Zombies: Bereits gebundene Almanachmodelle werden beim Eintritt und Austritt der Debug-Sitzung aktualisiert. Gerufene Gruppentänzer beenden zuerst ihre native Auftauchbewegung und beginnen danach den normalen Wassereintritt. Gargantuars, Giga-Gargantuars, Tänzer, Gruppentänzer und Springteufel werden tiefer dargestellt. Gummienten verwenden die laufende Spine-Pose und ein natives Sprite-Material; ihre Größe folgt der tatsächlichen Körpergrafik. Importierte Sprites werden an der Wasserlinie beschnitten, einschließlich des Augen-Zombies. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Ursachen, Grenzen und manuelle Prüfung](references/pool-almanac-18-0-1/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 18.0.0

Zwei neue Fusionen: **Wegblastopf** (Flower Pot + Blover) und **Angsttopf** (Flower Pot + Scaredy-shroom), jeweils in beiden Karten- und Handschuhrichtungen. Originalgrafik, Almanach, Düngerbehandlung und gespeicherte Angriffs-/Projektilboni ergänzt. Football-, Springteufel-, Tanz-, Gruppentänzer-, Yeti-Zombie und beide Gargantuars dürfen in vorhandenen Pool-Leveln auch Wasserreihen nutzen. Diese Zombies und der Augen-Zombie erhalten dort automatisch eine originale Gummiente und den nativen Wasserübergang. Debug-Almanach einschließlich bisher gesperrter Einträge freigegeben, ohne Profiländerung. [Belege, Anpassungen und Benutzerprüfung](references/support-pots-pool-18-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 17.0.4

Stand **17.0.4**: Greifalge stößt Zombies nach erreichtem Bisskontakt nicht mehr aus der nativen Bissreichweite. Rückstoß auf Distanz, Schaden und Wassergras bleiben erhalten. Die Prüfung berücksichtigt auch Zombies, deren nächster nativer Fress-Tick noch aussteht. [Ursache und Benutzerprüfung](references/grapple-bite-17-0-4/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 17.0.3

Stand **17.0.3**: Die gemeinsame Angstprüfung ist von Schadenskategorien getrennt. Auch nahe Gegner außerhalb der Bodenkategorie unterbrechen Schüsse in beiden Nachbarreihen und lösen die vollständig geladene Rasereipilz-Explosion aus. Greifalge und Algennuss verwenden bei der nativen Zielprüfung eine neutrale, fressbare Wasserpflanzenidentität. Der Squash-Zombie erhält den nativen Poolübergang. Scharfertang verwendet das richtige Originalprefab und spielt seinen Griff auch bei nativ initialisierten oder geladenen Zielkoordinaten ab. [Belege und Benutzerprüfung](references/feedback-17-0-3/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 17.0.2

Stand **17.0.2**: Rasereipilz löst seine geladene Angstexplosion sofort bei Nähe aus, einschließlich Nachbarreihen. Scaredy-Fusionen unterbrechen bei nahen Zombies weiterhin ihre Schüsse. Slowdown übernimmt die tatsächlich berechnete Größe und den nativen Knopfhintergrund. Greifalge ist für Zombies fressbar. Die Originalprüfung bestätigt beim Dreifachtang einen verbrauchenden Griff; drei bezeichnet seine Schussreihen. [Ursachen und Benutzerprüfung](references/feedback-17-0-2/report.md). Spiel und Testprogramme werden nicht gestartet.

## Änderung 17.0.1

Stand **17.0.1**: Korrekturen an grauen Debug-Fusionskarten, Fusionsvorschauen, Rasereipilz-Zielreihen, Slowdown-Hintergrund, originalem Gloom-Rauch, Greifalgen-Geometrie und Meersonnenpilz-Größe. Dreifachtang verbraucht sich nach dem einmaligen Tanggriff wie im Original. Hypnoweher wirkt ausschließlich auf bereits hypnotisierte Zombies. Meeresnuss und Meerkürbis bleiben nach Benutzerbestätigung unverändert. [Ursachen, Prüfung und Rückbau](references/feedback-17-0-1/report.md). Spiel und Testprogramme werden nicht gestartet.

## Änderung 17.0.0

Stand **17.0.0**: Benutzerkorrekturen an Schuss-/Statuseffekten, Verderbsen-Synergie, Kürbis-Handschuhplatzierung, Reihenfeuer, Tang, Kopfphysik und Wasserpflanzengrafik. Meerkürbis darf jetzt ohne Seerose auf Wasser stehen; deshalb erhöht sich die Gameplaynummer. [Änderungen, vorhandene Korrekturen und Benutzerprüfung](references/feedback-17-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 16.0.0

Stand **16.0.0**: Beim nativen Threepeater feuert der Dreifachtopf seine zwei Zusatzgeschosse jetzt 0,2 Sekunden nach der sechsten Haupterbse. So fliegen die Zusatzgeschosse zeitlich hinter der normalen Salve. Die Verzögerung verwendet gespeicherte Spielticks und pausiert mit dem Spiel. Schussgrenze und halber Zusatzschaden bleiben erhalten. Das Benutzerlog bestätigt, dass die Zusatzgeschosse bereits in 15.0.7 erzeugt wurden. [Verhalten, Belege und Benutzerprüfung](references/tri-pot-delay-16-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 15.0.4

Stand **15.0.4**: Enflamed färbt am tatsächlichen nativen Controller-Farbschreiber orange. Der spätere Zeichenpfad kann die Statusfarbe dadurch nicht mehr mit seiner Grundfarbe überschreiben. Alter Farbzustandscache entfällt; Gift nutzt denselben korrigierten Weg. [Ursache und Benutzerprüfung](references/enflamed-tint-15-0-4/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 15.0.3

Stand **15.0.3**: Der zusätzliche Nachtpool ohne Nebel verwendet jetzt den nativen ClearAlpha-Zeichenpfad statt eines dauerhaft gesetzten Blover-Countdowns. Normaler Nachtpool bleibt mit Nebel. [Ursache und Benutzerprüfung](references/debug-no-fog-15-0-3/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 15.0.2

Stand **15.0.2**: Große Debug-Zombiekarten verwenden ihr sichtbares Bild wieder als klickbare Buttonfläche. Orange Auswahlfärbung, Vorschau und Spawn erhalten dadurch wieder eine ausgewählte Zombieidentität. [Ursache und Benutzerprüfung](references/debug-zombie-cards-15-0-2/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 15.0.1

Stand **15.0.1**: Debug-Eingabe im Hauptmenü verwendet den nativen Levelauswahlzustand statt der GameObject-Aktivität. Tastendrücke werden am Anfang von LateUpdate gelesen; parallele Menümerker entfallen. [Ursache und Prüfhinweise](references/debug-input-15-0-1/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 15.0.0

Stand **15.0.0**: Umfangreiche Korrekturen an Debug-Eingabe und Platzierung, Flugpflanzen, Minen, Tang, Schusszählern, Projektilen, Originaleffekten, Statusfarben, Strahlmündungen und Renderreihenfolgen. Zusätzlicher Nachtpool ohne Nebel; Fast-forward mit 5x, 10x, 20x und Rechtsklick zum Herunterschalten. Keine neuen Pflanzen. [Änderungen, Belege und Benutzerprüfung](references/bugfixes-15-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 14.0.6

Stand **14.0.6**: Die Größenreferenz des Augen-Zombies enthält jetzt den vollständigen nativen Standardkopf einschließlich Kiefer und Haaren. Eine frühe Pose ohne Kopf hatte trotz korrekter Quellmessung erneut eine zu kleine Zielhöhe geliefert. Die Messung liest die nativen Kopf-Attachments ohne Änderungen am Spielzustand. [Ursache und Belege](references/eyes-complete-height-14-0-6/report.md). Spiel und Testprogramme bleiben beim Benutzer.

## Änderung 14.0.5

Stand **14.0.5**: Die Höhenmessung des Augen-Zombies berücksichtigt nur sichtbare Körperteile. Deaktivierte Effekt- und Kopf-Sprites hatten die gemessene Quellhöhe vergrößert und das sichtbare Modell verkleinert. Der Maßstab folgt weiterhin der echten nativen Körperhöhe. [Messbelege und Benutzerprüfung](references/eyes-visible-height-14-0-5/report.md). Spiel und Testprogramme bleiben beim Benutzer.

## Änderung 14.0.4

Stand **14.0.4**: Der Augen-Zombie verwendet beim Zugriff auf den nativen Spine-Renderer die nichtgenerische Unity-Schnittstelle. Das umgeht den im Benutzerlog belegten IL2CPP-Constraintfehler, durch den das Originalmodell verworfen wurde. Die Messung der nativen Körperhöhe einschließlich Datenasset-Skalierung bleibt erhalten. [Fehlerbeleg und Prüfung](references/eyes-interop-14-0-4/report.md). Spiel und Testprogramme bleiben beim Benutzer.

## Änderung 14.0.3

Stand **14.0.3**: Augen-Zombie misst die aktuelle native Spine-Geometrie einschließlich des zuvor übersehenen Datenasset-Faktors 0,564. Keine Abhängigkeit von lesbaren Mesh-Puffern und kein unskalierter Größen-Ersatzwert mehr. [Ursache, Schnittstellen und Benutzerprüfung](references/eyes-scale-14-0-3/report.md). Spiel und Testprogramme bleiben beim Benutzer.

## Änderung 14.0.2

Stand **14.0.2**: Topf-Tempoboni erhalten laufende Schussanimationen und speichern ungenutztes Guthaben. Schutz für alle Glut-/Kirschschützen einschließlich Split und Gatling; eigene Fusionssalven zusätzlich geprüft. Augen-Zombie misst sichtbare native Mesh-Dreiecke statt von unsichtbaren Locator-Punkten verfälschte Renderer-Bounds; sein Originalmodell bleibt bei nicht lesbaren Meshes mit belegter Körperreferenz erhalten. [Ursachen, betroffene Pflanzen und Benutzerprüfung](references/repeater-eyes-14-0-2/report.md). Spiel und Testprogramme bleiben beim Benutzer.

## Änderung 14.0.1

Stand **14.0.1**: Augen-Zombie misst Körperhöhen in der lokalen Sprite-Ebene und begrenzt ungültige Skalierungen. Eisenstern, Chilistern, Sternnuss und Sternkappen starten oberhalb der nachgewiesenen nativen Boden-Grenze; Chilistern zeigt natives Reihenfeuer und schmilzt Eisfahrspuren. Gemeinsamer Schadensmaßstab am finalen Trefferweg: 20/20 = 1,0, einschließlich Flächenangriffen, Strahlen und Gift. Feuertopf-Almanach nennt alle Feuerempfänger und erklärt, weshalb Faulpilz und Dreifachmatschkürbis keinen Bonus erhalten. [Ursachen, Quellen und Prüfung](references/regressions-14-0-1/report.md). Spiel und Testprogramme bleiben beim Benutzer.

## Änderung 14.0.0

Stand **14.0.0**: Dreifach-Laternenblatt und Polarlicht-Laternenblatt mit Originalassets, Licht-/Laserfähigkeiten, Dünger und DE/EN-Almanach. Korrigiert: Bissschadensmaßstab, Sternuhren/Chilistern-Tod, Feuertopf-Jalapeno, horizontale Original-Laser, Faulpilzrauch/Augen, Augen-Zombie-Größe/Frost, Katapult-Einschlag und Hüpfexplosionsgrafik. Debug-Pflanzennamen ohne führendes `$`. [Belege, Schadensvergleich und Benutzerprüfung](references/fusion-repairs-14-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 13.1.0: Phoenix-Triplinator, Solar-Katapult und Fehlerkorrekturen

- Neu: Charred Threepeater + Jalapeno = Phoenix Threepeater; Cabbage-pult + Sunflower = Solar-pult. Beide Quellenrezepte sind vollständig registriert, einschließlich Originalmodellen, Geschossen, Fähigkeiten, Speicherung, Dünger und deutschem/englischem Almanach.
- Topfauswahl im unteren Felddrittel ergänzt. Tempo-/Zitterpilz verwenden originale grüne/blaue Sporen. Katapulte teilen eine korrigierte Flugberechnung.
- Augen-Zombie behält sichtbare Butter und erhält die Größe des normalen Körpers. Hochwall-Nuss verwendet ihr echtes Originalporträt. Apokalyptoblatt zeigt originales DoomSplat statt FumeCloud.
- Aschfahler Triplinator erwacht durch Rezept oder Feuerreihe als Phoenix. Chilistern schließt seine einmalige Sternsalve am Zündtimer ab. Lichterbsen verwenden Sprite-Laser ohne den im Benutzerlog belegten LineRenderer-Fehler. Doppelsolar-Nuss behält ihre Solarballgrafik.

[Quellen, Grenzen, Kompilierungsbelege und manuelle Prüfliste](references/fusion-repairs-13-1-0/report.md). Spiel und Testprogramme wurden vom Agenten nicht gestartet. Workflow bleibt unverändert.

## Änderung 13.0.9: native Zombievarianten und korrekte Raster-Lane

- Die sechs Pflanzenkopf-Zombies verwenden offline aus nativen Kopf-Skeletten gerenderte Bilder einschließlich Zombotany-Texturen, Skins und Prefabversätzen. Ganze Pflanzen mit Stiel werden nicht mehr auf beschnittene Körper gesetzt.
- Giga-Gargantuar erhält seinen echten Rotaugen-Skin; Zielzombie sein vollständiges natives Zielmodell mit intakten Armen und Schild. Acht korrigierte Varianten stehen in Karten und Rastervorschauen aus demselben Atlas bereit. Der persönliche Zombatar bleibt beim gemeinsamen statischen Standardkörper.
- Rastervorschauen verwenden die Fußlinie ihrer tatsächlichen Reihe statt der Oberkante. Die Zeilenhöhe stammt aus dem Board: 100 beziehungsweise 85 Pixel. Dachneigung bleibt nativ berücksichtigt. Augen-Zombie und native Zombie-Sprites verwenden denselben unteren Bildanker.

Fixrelease **13.0.9**. [Quellen, Darstellungsgrenzen, Prüfung und Release](references/zombie-accuracy-13-0-9/report.md). Nur Offline-Assetkonvertierung, Dateiansicht und Kompilierung; Spiel und Testprogramme wurden durch den Agenten nicht gestartet. Die bereits laufende Benutzersitzung lädt diese DLL erst nach vollständigem Neustart.

## Änderung 13.0.8: Kartengrafiken über Szenenwechsel erhalten

Unitys Szenenbereinigung konnte die vorab geladenen Atlas-Sprites und Texturen entfernen, obwohl ihre C#-Cacheeinträge weiter existierten. Dadurch blieben Fusionen/Zombies ohne Grafik; der Augen-Zombie brach anschließend den Modellaufbau mit einer NullReferenceException beim Lesen der zerstörten Hintergrundtextur ab.

Alle selbst erzeugten Kartenressourcen erhalten jetzt unmittelbar bei Erzeugung DontUnloadUnusedAsset. Der Sitzungsbesitzer gibt sie weiterhin ausdrücklich frei. Das umfasst Fusionsatlas, Zombieatlas, sämtliche Spritebereiche, Kartenhintergrund und Augen-Zombie-Grafiken. Atlas-/Auswahlarchitektur und die Entfernung der instabilen Live-Aufnahmen bleiben erhalten.

Fixrelease **13.0.8**. [Logbelege, Lebenszyklus und manuelle Prüfung](references/chooser-assets-13-0-8/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 13.0.7: Zombie-Karten ohne Live-Aufnahmen

Der zweite Abbruch trat erneut unmittelbar nach Beginn der Aufnahme Normal auf, diesmal bei vollständig sichtbarer Auswahl. Die spätere Freigabe aus 13.0.6 beseitigte den Fehler nicht. Der Live-Aufnahmeweg ist jetzt vollständig entfernt, einschließlich seiner Initialisierungs-/Reanimationspatches. Karten erzeugen keine Zombies, Almanachbinder, Kameras oder RenderTextures mehr.

Alle 37 nativen Zombietypen besitzen vorab exportierte Rasterbilder und vollständige Karten im eingebetteten Atlas. Die Darstellung steht sofort bereit und benötigt keinen ersten Cachefülllauf. Varianten ohne eigene statische Vorschaugrafik haben eine ausdrückliche Grenze: sechs Pflanzenkopf-Zombies kombinieren Originalkörper und Originalpflanzengrafik; Rotaugen-Gargantuar verwendet das Gargantuarbild, Zombatar das normale Körperbild. Namen und tatsächliche Spawnidentitäten bleiben getrennt. Augen-Zombie behält seine eigene Originalgrafik.

Fixrelease **13.0.7**. [Abbruchbefund, Quellen, Darstellungsgrenzen und Prüfung](references/chooser-crash-13-0-7/report.md). Spiel und Testprogramme wurden nicht gestartet. Der verdächtige native Erzeugungspfad ist entfernt; Laufzeitbestätigung bleibt beim Benutzer.

## Änderung 13.0.6: Porträtaufnahme erst nach abgeschlossener Menüinitialisierung

Das Log des Benutzerlaufs mit 13.0.5 endet direkt nach der ersten Porträtaufnahme, nur 82 ms nach der Board-Bereitmeldung. Ein vorhandenes Board genügt nicht als Freigabe für native Darstellungsobjekte. Porträtaufnahmen warten jetzt auf das gebundene, aktive und vollständig eingeblendete native Auswahlpanel im normalen Auswahlzustand; während Übergängen laufen sie nicht. Im gestarteten Spiel ist weiterhin Vorladen möglich. Cache, Kartenatlas und Vorschaufunktionen bleiben erhalten.

Fixrelease **13.0.6**. [Abbruchbelege, Änderung und Prüfgrenze](references/chooser-crash-13-0-6/report.md). Ohne Crashdump ist die genaue native Absturzursache nicht bewiesen. Spiel und Testprogramme wurden nicht gestartet.

## Änderung 13.0.5: Kartencache, vollständige Rahmen und Fusionsvorschau

- Alle 291 Fusions-/Objektkarten stehen in einem vorab erzeugten Atlas mit vollständigem Kartenrahmen. Beim Blättern werden keine einzelnen Porträt-PNGs mehr decodiert. Der bisherige Mesh-Callback erzeugt keine UI-Objekte mehr.
- Auswählen/Entfernen behalten die Rasterliste des aktuellen Tabs. Die Samenleiste meldet ihre vollständige Änderung einmal statt nach jedem Paket.
- Zombieporträts werden während der Pflanzenauswahl vorbereitet und unter UserData/FusionMod/zombie-portraits-v2-<native SHA256>/ gespeichert. Weitere Spielstarts verwenden den Cache. Eine erste Erzeugung bleibt nötig. Ungültige Aufnahmen werden nicht gespeichert. Zombie-Karten erhalten denselben Rahmen; die Rastervorschau verwendet das Bild ohne Rahmen.
- Führende Dollarzeichen werden aus den lokalisierten Zombie-Namensbestandteilen entfernt.
- Fusions-/Objektvorschauen an Maus und Raster verwenden die genaue Kartenidentität und den vorhandenen importierten Modellrenderer. Wiederverwendete Controller werden auch beim Wechsel zwischen Fusionen gleichen Grundtyps aktualisiert. Dafür entstehen keine Spielpflanzen oder Fähigkeiten.
- Weitere Logkorrekturen: Fokus vor Freigabe der Bindermodelle lösen; Dreikopf-Alpha über shoot1/2/3 statt einen fehlenden shoot-Clip auswerten; Porträtkontext vom lebenden Debug-Board beziehen.

Fixrelease **13.0.5**. [Belege, Grenzen und manuelle Prüfung](references/chooser-fix-13-0-5/report.md). Spiel und Testprogramme wurden nicht gestartet. Nach Änderungen an den Almanachporträts erzeugt tools/build_debug_cards.py den Atlas neu; die statische Prüfung erkennt veraltete Quellen.

## Änderung 13.0.4: Kartenidentität, Samenleiste und Seitenleistung

- Die Auswahl verwendet die konkrete Karte statt ihres mehrfach verwendeten Grundpflanzentyps. Fusionen landen dadurch in der nativen Samenleiste. Entfernen läuft ohne rekursiven Aufruf der beiden nativen Überladungen; die restlichen Karten werden über den nativen Bankallocator lückenlos neu zugeordnet. Debug-Karten werden unmittelbar zugeordnet; die bisherigen Kartenflüge entfallen in dieser Auswahl.
- Gewählte Karten gehören ausschließlich zur Samenleiste und werden beim Blättern nicht erneut ins Auswahlraster eingefügt. Die Kartenmodelle bleiben zwischen Tabs bestehen. Änderungen an den nativen Modelllisten werden gebündelt gemeldet; identische Listen erzeugen keine erneute Bindung. Auch die unveränderte Imitatorliste wird nicht bei jedem Tab neu erzeugt.
- Fusionsporträts werden höchstens einzeln pro Frame decodiert, statt gesammelt im Seitenklick. Bis dahin bleibt der native Kartenhintergrund sichtbar. Fusionen und Zombies erhalten denselben nativen leeren Kartenhintergrund, Platz für Kosten und unverzerrte Porträts. Das gilt auch für ihre Karten in der Samenleiste.

Fixrelease **13.0.4**. [Ursachenbelege, Änderungen und manuelle Prüfung](references/chooser-fix-13-0-4/report.md). Spiel und Testprogramme bleiben geschlossen.

## Änderung 13.0.3: stabile Auswahl und erreichbare Kartenpfeile

- Die Debug-Auswahl setzt Kartenpositionen nur beim Seitenwechsel und bei Zustandsübergängen. Native Binder behalten die Kontrolle über Bildschirmkoordinaten; die bisherige Rückkopplung mit dem Mod führte trotz behobenem Indexfehler weiterhin zum dauernden Kartenneuaufbau.
- Die Seitenpfeile verwenden direkt den nativen Beschleunigungsbutton einschließlich seiner geladenen Prefabinstanzen. Sie benötigen dessen erst in `Awake` gesetzten Button-Cache nicht mehr. Beide stehen in passender Canvasgröße über dem stabilen Imitatorrahmen. Panel und Kartenanker werden über ihren eigenen Modellschlüssel aufgelöst.
- Unsichtbare Zusatzseiten erzeugen keine Kartenmodelle oder Porträtabonnements. Ausgewählte Karten bleiben erhalten. Zusätzliche Karten erhalten dieselbe Flugzeitfortschreibung und denselben nativen Landeabschluss wie die ursprünglichen 49 Karten.

Fixrelease **13.0.3**. [Befund, native Belege und manuelle Prüfliste](references/chooser-fix-13-0-3/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 13.0.2: Auswahlmenü nach Logfehler wieder freigeben

Die Debug-Auswahl verwendet jetzt durchgängig `NumSeedsInChooser` (49) für native Karten, Imitator, Seitenbeginn und den Wechsel zur Zombieauswahl. `NumSeedTypes` (53) enthält zusätzlich vier Sonderpflanzen ohne Auswahlkarte. Der bisherige Imitatorweg las dadurch außerhalb seiner 49 Einträge; der abgebrochene Modell-Tick ließ die Auswahl schmutzig und baute Karten samt Tweens ständig neu auf. Die ersten vier Zusatzkarten erhalten jetzt außerdem ihre richtige Tabposition. Native Karten, Tabs und Imitator bleiben erhalten.

Fixrelease **13.0.2**. [Ursachenbelege, Prüfung und verbleibende Grenzen](references/chooser-fix-13-0-2/report.md). Spiel und Testprogramme wurden nicht gestartet.

## Änderung 13.0.1: native Debug-Kartentabs und Logkorrekturen

- Die zusätzliche grüne Pflanzenauswahl und die getrennte Fusionsleiste entfallen. Alle Karten stehen jetzt im vorhandenen nativen Feld **„Wähle deine Pflanzen“** und landen in derselben nativen Samenleiste. Tab 1 enthält die normalen Pflanzen; ab Tab 2 folgen Objekte und Fusionen mit Originalporträts und ihren berechneten Sonnenkosten.
- Zwei aus dem nativen Fast-Forward-Knopf abgeleitete Richtungsknöpfe stehen über dem Imitator. Vor-/Zurückblättern erhält die gewählten Karten und wartet laufende Kartenflüge ab. Bestätigen und anschließende Zombieauswahl bleiben nativ. Es gibt weiterhin höchstens zehn Pflanzen-/Fusions-/Objektkarten.
- Der Logfehler `ModelBase being disposed multiple times` wird an seiner Ursache behandelt: Die gefilterte Kartenliste verwendet gemeinsame Referenzen statt eines zweiten Besitzers derselben Kartenmodelle.
- Zombieporträts verwenden den tatsächlich erzeugten nativen Zombiecontroller statt des unbesetzten Binderfelds. Pflanzenkopf-/Zombatar-Initialisierung erhält einen begrenzten App-Kontext; dabei erzeugte Kopfreanimationen bleiben unter dem isolierten Almanachbinder. Präsentationsobjekte werden ohne den für sie ungültigen Gameplay-Todespfad entfernt.
- RenderTextures werden zusätzlich an ihrer tatsächlichen Freigabegrenze von noch verbundenen Kameras getrennt; die bestehende Behandlung beim Beenden bleibt erhalten.

Fixrelease **13.0.1**. [Logbefund, native Belege, Grenzen und manuelle Prüfliste](references/native-debug-cards-13-0-1/report.md). Spiel und Testprogramme wurden nicht gestartet. Workflow, Fusionenliste und Rezeptquelle bleiben unverändert.


## Änderung 13.0.0: Debug-Tabs, Leuchtklee, Rasereipilz und Pilzkorrekturen

- Neu: **Leuchtklee / Glowver** und **Rasereipilz / Frenzy-shroom** mit sämtlichen verfügbaren Quellenrezepten, Originalmodellen und Animationen, Fähigkeiten, Projektilen, Düngerbehandlung und vollständigem deutschem/englischem Almanach. Beide erledigten Einträge sind aus `fusionen.txt` entfernt. Rasereipilz verwendet das belegte Mindestintervall von **0,2 Sekunden** und übernimmt den Schaden einer unmittelbar rechts stehenden Unheilerbse.
- Debug-Pflanzenauswahl besitzt die Tabs **Pflanzen**, **Fusionen** und **Objekte**. Alle 288 Almanachergebnisse stehen als Fusionskarten zur Wahl. Fusionskosten verwenden den günstigsten vollständigen verfügbaren Rezeptweg; lose Zutaten kosten jeweils **100**. Unterstützte Objekte sind Eimer, Football-Helm und Augen-Zombie-Kopf. Insgesamt sind zehn Pflanzen-/Fusions-/Objektkarten möglich; Zombies werden anschließend getrennt gewählt. Gold-Aufwertungen berücksichtigen ihre erste Bohnenzahlung. Kampfgeld und Sonne bleiben im Debug-Level unbegrenzt.
- Mini-Eispilz prüft Kontakt im Fähigkeitstakt und friert sein Bodenziel ohne Zusatzschaden ein. Mini-Hypnopilz wird beim Augen-Zombie durch dessen eigenen Bisszeitgeber ausgelöst. Geschrumpfte Zombies erhalten eine Korrektur für die native vertikale Bitmapverschiebung; die importierte Augen-Zombie-Grafik folgt der verkleinerten Silhouette.
- Ersetzte Pilz-Fähigkeitspfade behalten den Kaffee-Aufwachzähler einschließlich Aufwachton. Das betrifft insbesondere Sonnenspeicherpilz und Eiscremepilz.
- Rußpilz verwendet den ursprünglichen schwarzen Rauch **FumeDoom**, einschließlich dessen Partikelgröße, Lebensdauer, Geschwindigkeit und Emissionsrate. Eiszapfenpilz besitzt die volle Reihenreichweite seines tatsächlichen Quell-Elterntyps und erzwingt seine Eisprojektilgrafik nach der Kennzeichnung.
- Unheilsporen wachsen um ihren Bodenanker statt um die Szenenposition des exportierten Prefabs. Die Darstellung ist kleiner. Hypnoerbsen-Zombies tragen den animierten Originalkopf der Hypnoerbse. Zombie-Platziervorschauen verlieren den zusätzlichen vertikalen Versatz von 65 Pixeln. Der Augen-Zombie übernimmt Kälte als strukturerhaltende Farbtönung.

Neue Debug-Spielregeln verwenden die erste Versionsstelle: **13.0.0**. [Quellen, genaue Grenzen, Buildbelege und manuelle Prüfliste](references/debug-fusions-13-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet. Workflow unverändert.


## Änderung 12.0.0: Parfümpilz, Tangstachel und Korrekturen

- Neu: **Parfümpilz / Perfume-shroom** und **Tangstachel / Tangle Weed**, mit sämtlichen verfügbaren Quellenrezepten, Originalmodellen/-animationen, Rauchprofil, Fähigkeiten, Düngerbehandlung und deutschem/englischem Almanach. Beide erledigten Einträge sind aus `fusionen.txt` entfernt.
- Kaffee weckt alle drei Pilze derselben Zelle. Bei Topfrezepten fusioniert das untere Drittel den Topf; oben wird normal gepflanzt oder der Bewohner fusioniert. Topffusionen erscheinen kleiner.
- Tempo-/Zitterpilz-Projektile verwenden den animierten Schusspunkt und passende Geschossgrafiken, auch nach Torchwood. Giant Wall-nut unterdrückt normale Zombie-Todesanimationen nicht mehr.
- Debug-Zombieauswahl: ausschließlich Zombiekarten, native vollständige Variantenporträts, unverzerrte Bilder und Namen aus der aktuellen Spielübersetzung.
- Letzter roter Logfehler `Releasing render texture that is set as Camera.targetTexture!`: Kameraziele werden bereits im nativen `Application.quitting` getrennt, bevor Renderer Texturen freigeben.

Gameplayklasse: **12.0.0**. [Quellen, Fähigkeiten, Buildbelege und manuelle Prüfliste](references/fusions-fixes-12-0-0/report.md). Spiel und Testprogramme wurden vom Agenten nicht gestartet; insbesondere neue Debug-Porträts und Abschaltreihenfolge bleiben zur Laufzeit zu prüfen. Workflow unverändert.

## Änderung 11.0.0: vier Minenfusionen, Slowdown und HUD

- Neu: **Tangle Mine**, **Kelp Mine**, **Sea-mine** und **Potato Masher** mit allen verfügbaren Quellenrezepten, Originalgrafiken/-animationen, Fähigkeiten, Düngerbehandlung und deutschem/englischem Almanach. Vier erledigte Einträge sind aus `fusionen.txt` entfernt. Keine dieser Fusionen schießt Projektile; Potato Mash ist ein nativer Squash-Ableger mit Originalgrafik.
- Richtung beachten: Schlingtang auf Kartoffelmine ergibt Tangle Mine; umgekehrt Kelp Mine. Kartoffelmine auf Meerpilz ergibt Sea-mine; umgekehrt bleibt Land Mine. Potato Masher funktioniert in beiden Richtungen.
- Geldanzeige und Goldbohne stehen oberhalb von Fast Forward. Im Debug-Level ist Kampfgeld unendlich, beide Anzeigen zeigen ∞; Profil-/Shopgeld bleibt erhalten.
- Neuer Slowdown neben Fast Forward: gespiegeltes originales 1,5×-Symbol, Toggle mit sichtbarer Kennzeichnung, exakt **0,2×**. Ausschalten übernimmt die aktuell gewählte Fast-Forward-Geschwindigkeit. Pause bleibt nativ.
- Marigold- und Flowerpot-Projektilfehler: eigene Produktionszähler gelangen nicht mehr in den nativen Schussweg. Fehlgeschlagene Debug-Zombies werden einschließlich Teilinstanzen sauber abgebrochen; nicht verfügbare Spezialtypen zeigen einen Hinweis. Beim Beenden werden Kamera-RenderTextures vor der Freigabe getrennt.

Gameplayklasse: **11.0.0**. [Quellen, genaue Fähigkeiten, Richtungen, Buildbelege und manuelle Prüfliste](references/mines-11-0-0/report.md). Spiel und Testprogramme wurden vom Agenten nicht gestartet. Workflow unverändert.

## Änderung 10.0.0: Goldbohne, zwei Gold-Aufwertungen und native Debug-Minispielansicht

- Das originale Goldbohnen-Icon steht neben dem Geldzähler. Aufgesammelte Münzen füllen zusätzlich einen gespeicherten Levelvorrat; Profil-/Shopgeld wird nicht ausgegeben. Bohne wählen, dann Silberpflanze oder Goldpflanze anklicken. Rechtsklick und Escape brechen ohne Verbrauch ab.
- Die bestätigten zwei Aufwertungen sind Goldnuss und Goldsonnenblume. Silbernuss kostet 1000 einschließlich der ersten Heil-/Verstärkungsfähigkeit. Silbersonnenblume kostet zuerst 3000, verdoppelt je vorhandener Goldsonnenblume. Weitere Fähigkeiten kosten 1000: Nuss stärkt und heilt sich; Sonnenblumen zahlen gemeinsam ihre gespeicherte Sonne aus. Im Debug-Level sind diese Aktionen kostenlos. Originalmodelle, Animationen, Porträts und englische/deutsche Almanachtexte sind enthalten. Beide besitzen keine Projektile.
- Dünger heilt beide Goldpflanzen. Bei der Goldsonnenblume verkürzt er zusätzlich die nächste Produktion auf höchstens 0,5 Sekunden; er kauft keine Fähigkeit und wandelt keine Silberpflanze in Gold um.
- Debug verwendet die originale native Minispielvorlage mit Scrollraster und Hintergrund-Vorschaubildern. Pflanzen-/Zombieauswahl und Intro bleiben im vorhandenen nativen Ablauf. Vorlage und Profildaten werden nicht verändert.

Neue Gameplayregel durch bezahlte Gold-Aufwertungen: **10.0.0**. `fusionen.txt` bleibt korrekt unverändert, weil diese Werkzeug-Aufwertungen dort nicht als Pflanzenpaar-Rezepte stehen. Offene weitere Fusionen mit Goldzutaten bleiben erhalten. [Quellen, Bedienung, Speichervertrag und manuelle Prüfliste](references/golden-bean-10-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet. Workflow bleibt unverändert. Vorherige DLL: `backup/FusionMod-9.0.0.dll`; vorherige Quellen: `backup/golden-bean-9.0.0.tar.gz`.

## Änderung 9.0.0: Fusionswirkungen, Pilzgrafiken und zuverlässige Augen-Zombie-Bisse

- Wird ein echter Ice-shroom bei einer erfolgreichen Fusion verbraucht, tritt seine globale Eiswirkung ein. Karten- und Handschuhfusionen verwenden denselben Abschluss. Bei mehreren kleinen Ergebnissen wirkt eine eingehende Ice-shroom-Karte genau einmal. Vorhandene Eis-Speicherung im Cryo-Kürbis und Frostnuss-Heilung verwenden ihre bestehenden Regeln. Laden und fehlgeschlagene Fusionen lösen nichts aus.
- Mutpilzrauch entsteht am originalen animierten `Shoot`-Punkt statt am versetzten nativen Controllerursprung. Alle direkten Stinkpilz-Rauchvarianten verwenden nun Originalsprites und Emissionsprofile: darunter Stinkmine, Eis-, Feuer-, Magnet-, Eisen- und Stachelrauch. Mehrreihige Strahlen erhalten den passenden Reihenversatz. Schaden bleibt im vorhandenen Simulationsweg.
- Alle acht Angstpilz-Fusionen verwenden dieselbe Umrechnung von Originalmaßen zu Spielfeldmaßen. Tempo-, Angstbomben-, Kartoffel-, Sonnen-, Stern-, Nuss- und Tarnvarianten werden damit an die bereits passende Eisvariante angeglichen. Die dazugehörigen Erbsen-, Kirsch-, Kartoffel- und Eisprojektile verwenden denselben Maßstab; Mutpilzsporen ebenfalls.
- Beim Zitterpilz und den anderen Angstpilz-Schützen genügt eine bereits gültige allgemeine Erbsenansicht nicht mehr: Die Ansicht muss zum gespeicherten Projektiltyp passen. Allgemeine Erbsengrafiken beanspruchen keine markierten Angstpilz-Projektile.
- Der Augen-Zombie verliert keine Bissereignisse mehr an der nativen Zielprüfung im Abstand von vier beziehungsweise acht Ticks. Nur während dieser Prüfung wird der Alterstakt passend gesetzt und anschließend wiederhergestellt. Schaden bleibt 50 pro Biss; schnelle Bissanimation, Pause, Kälte, Butter und Eis behalten ihre Zuständigkeit.
- Der Frostschnapper beginnt gegen ungefrorene Ziele erst innerhalb des tatsächlich schädigenden Eispulses einen Biss. Gefrorene verschluckbare Ziele bleiben innerhalb der längeren Fressreichweite zulässig. Angriffsbeginn und Eispuls verwenden dieselbe geometrische Prüfung.
- Minenfusionen verwenden den nativen Kartoffelminen-Explosionseffekt statt der Kirschgrafik. Das gilt auch für weitere Fähigkeiten, die denselben Minenexplosionshelfer verwenden.

Gameplayänderung durch die Ice-shroom-Fusionswirkung: **9.0.0**. Keine neuen Pflanzen, Zombies oder Rezepte. [Belege und manuelle Prüfliste](references/fusion-effects-9-0-0-native/report.md). Spiel und Testprogramme wurden nicht gestartet. Vorherige DLL: `backup/FusionMod-8.0.0.dll`; Quellen- und Ressourcenarchive liegen unter `backup/fusion-effects-8.0.0.tar.gz` und `backup/fume-mine-effects-8.0.0.tar.gz`. Ergänzende Originaldateien: `backup/ScaredContent-8.0.0.cs` und `backup/ChomperVariants-8.0.0.cs`.

## Änderung 8.0.0: Debug-Platzierung und Augen-Zombie-Grafik

- Im Debug-Level fällt keine automatische Sonne vom Himmel. Pflanzensonne bleibt Teil der jeweiligen Pflanzenfunktion. Diese Gameplayänderung erhöht die erste Versionsstelle auf **8.0.0**.
- Die Zombieplatzierung zeigt ein halbtransparentes Zombiebild am aktuellen Rasterfeld. Die Vorschau erzeugt keine echte Einheit und verschwindet außerhalb gültiger Felder, bei Abbruch, Pflanzenwahl, Pause und Levelende.
- Die Debug-Zombieleiste wird während der Pause ausgeblendet und verarbeitet dort keine Boardklicks.
- Nach der Zombieauswahl werden die nativen Pflanzenmodelle samt Bildreferenzen ausdrücklich wiederhergestellt. Gleiche numerische Pflanzen- und Zombieslots behalten dadurch kein Zombiebild in der Pflanzenleiste.
- Das Leeren der Auswahl setzt leere Karten direkt zurück. `SetPacketType(None, None)` rief zuvor `Plant.GetRefreshTime` ohne Pflanzendefinition auf und erzeugte im letzten Log 40 `NullReferenceException`-Meldungen. Verborgene Chooser-Slots einschließlich Imitater bleiben für die nativen Binder vorhanden; die dort protokollierten fehlenden `all.48`-Modelle werden damit adressiert.
- Der abfallende Kopf des Augen-Zombies verwendet dessen importierte Kopfgrafik auf dem nativen Kopfeffekt. Bewegung, Physik und Lebensdauer bleiben nativ. Der separate 1-%-Kopfdrop bleibt unverändert.
- Die importierte Körpergrafik übernimmt native Farb- und additive Materialeffekte für Trefferblinken und Kälte; der Schatten erhält keinen Trefferblitz.

Keine neuen Pflanzen, Zombies oder Rezepte. [Ursachen und manuelle Prüfliste](references/debug-eyes-8-0-0-native/report.md). Nur statische Prüfung und Kompilierung; Spiel und Testprogramme wurden nicht gestartet. Vorherige Quellen: `backup/debug-eyes-7.0.0.tar.gz`; vorherige DLL: `backup/FusionMod-7.0.0.dll`.

## Änderung 7.0.0: Debug ohne Pflanzen-Cooldown und robuster Auswahlwechsel

- Pflanzenkarten sind im laufenden Debug-Level sofort wieder verwendbar. Die Freigabe gilt auch für anfängliche Ladezeiten und Imitater. Native Pflanzung, Verbrauchszähler und Fusionsereignisse laufen weiter; normale Level behalten ihre Ladezeiten.
- Der Wechsel zur Zombieauswahl findet das tatsächlich mit dem aktuellen Chooser verbundene Panel über dessen native Datenbindung. Eine Namenssuche über alle geladenen, einschließlich inaktiven Panels entfällt.
- Der Wartepunkt berücksichtigt die effektive Sichtbarkeit: Verbirgt der native Zustandsbaum einen Elterncontainer, genügt dessen deaktivierte Hierarchie. Ein darin liegendes Panel muss seinen eigenen Zustand nicht auf Hidden ändern. Dadurch blockiert der Wechsel nicht mehr an diesem Zustand.
- Zombie-Karten starten an ihren nativen Rasterpositionen. Zuvor ausgewählte Pflanzen können ihre Kartenposition bereits in die obere Leiste verschoben haben; diese Positionen werden nicht mehr als Startpunkte der Zombieauswahl übernommen. Zwei Logeinträge markieren Pflanzenbestätigung und Öffnen der Zombieauswahl.

Gameplayänderung durch aufgehobene Ladezeiten: **7.0.0**. Keine neuen Pflanzen, Zombies oder Rezepte. [Belege und manuelle Prüfliste](references/debug-cooldown-7-0-0-native/report.md). Spiel und Testprogramme wurden nicht gestartet. Vorherige Projektdateien: `backup/debug-transition-cooldown-6.0.0.tar.gz`; vorherige DLL: `backup/FusionMod-6.0.0.dll`.

## Änderung 6.0.0: Native Debug-Auswahl und münzfreie Zombies

- Nach der Backgroundwahl startet der native Levelweg mit der originalen Pflanzenauswahl. Die zusätzliche selbst gebaute Pflanzenansicht entfällt. Anschließend verwendet die Zombieauswahl denselben nativen Chooser mit dessen Kartenflug, Auswahlleiste, Entfernen und Controllerbedienung.
- Der Phasenwechsel verwendet die native Panelanimation und wartet auf deren Ende. Nach der Zombieauswahl beendet der native Chooser die Auswahl; das normale Levelintro mit Kamerafahrt und „Ready, Set, Plant“ läuft weiter. Der bisherige direkte Aufruf von StartPlaying entfällt.
- Debug bietet unabhängig vom Profil zehn Plätze. Eine bis zehn Pflanzen und null bis zehn Zombies sind zulässig. Imitater behält den nativen Auswahldialog und den tatsächlich ausgewählten Imitationstyp. Profil und Spielstände werden nicht verändert.
- Zombiekarten im Level verwenden den leeren nativen Kartenhintergrund statt der farbigen rechteckigen Box. Zombiebilder und Varianten bleiben erhalten.
- Zombie.DropLoot ist nur für das Debug-Board gesperrt. Damit erzeugen getötete Zombies einschließlich Yeti keine Münzen oder andere native Beute.

Gameplayänderung: **6.0.0**. Keine neuen Pflanzen, Zombies oder Rezepte. `fusionen.txt` bleibt unverändert. [Native Belege, Grenzen und manuelle Prüfliste](references/debug-native-6-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet. Vorherige Projektdateien: `backup/debug-native-5.0.1.tar.gz`; vorherige DLL: `backup/FusionMod-5.0.1.dll`.


## Änderung 5.0.1: Vorschaubilder der Debug-Karten laden

- Pflanzen- und Zombiekarten weisen ihre Bildreferenz jetzt über `AddressableSpriteValueModel.Reference` zu, wie die normale native Kartenwahl. Der bisher verwendete Konstruktor ignoriert in dieser Spielversion sein Sprite-Argument und startet deshalb keinen Ladevorgang. Das erklärt die leeren Karten ohne Fehlermeldung.
- Die vorhandene UI übernimmt fertig geladene Bilder weiter asynchron. Dieselbe Korrektur gilt für Auswahlraster, obere Auswahlleiste, Pflanzenkopfsymbole und Zombiekarten im Level. Augen-Zombie-Bild und Ressourcenfreigabe bleiben unverändert.

Fix-Version **5.0.1**. [Native Belege und manuelle Prüfliste](references/debug-card-images-5-0-1/report.md). Spiel und Testprogramme wurden nicht gestartet. Vorherige DLL: `backup/FusionMod-5.0.0.dll`; vorherige Projektdateien: `backup/debug-card-images-5.0.0.tar.gz`.

## Änderung 5.0.0: Debug-Bildkarten, Werkzeuge und schneller Augen-Zombie

- Debug verwendet für Pflanzen und Zombies dieselbe Bildkartenansicht mit einer oberen Auswahlleiste wie bei einer normalen Kartenwahl. Erst Pflanzen auswählen und bestätigen, danach Zombies wählen. Beide Auswahlen sind auf zehn begrenzt; eine volle Auswahl lässt weitere Karten nicht zu. Gewählte Karten lassen sich oben wieder entfernen. Zurück zur Pflanzenwahl bleibt möglich.
- Zombiekarten zeigen auch im laufenden Debug-Level ihre Vorschauen. Alle zehn passen gleichzeitig in die Leiste. Native Pflanzen-/Zombie-Vorschaubilder laden asynchron und werden sitzungsweise freigegeben. Der Augen-Zombie verwendet ein aus seinem Originalmodell exportiertes Ganzkörperbild. Für native Varianten ohne eigenes Vorschaubild wird das native Körperbild verwendet; Zombotany-Karten erhalten zusätzlich ein Pflanzenkopfsymbol.
- Debug schaltet die native Schaufel frei. Der vorhandene Lawn-Glove-Mod verwendet denselben Schaufelknopf: Rechtsklick wechselt zwischen Schaufel und Handschuh; Linksklick aktiviert das gewählte Werkzeug. Profilfreischaltungen werden nicht verändert.
- Nach dem Öffnen seiner Augen und Abschluss der Aufwachanimation läuft der Augen-Zombie doppelt so schnell. Sein Fressclip und dessen schadenswirksame Bissereignisse laufen doppelt so schnell wie bisher. Schaden bleibt 50 pro Biss. Pause, Butter, Eis, Kälte und Hypnose bleiben native Regeln. Temporär skalierte Bewegungswerte werden unmittelbar nach dem nativen Laufaufruf wiederhergestellt.

Gameplayänderungen erhöhen die erste Versionsstelle auf **5.0.0**. Keine neuen Pflanzen, Zombies oder Rezepte; `fusionen.txt` bleibt unverändert. [Umsetzung, Quellen und manuelle Prüfliste](references/debug-cards-rage-5-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet. Vorherige DLL: `backup/FusionMod-4.0.0.dll`; vorheriger Projektstand: `backup/debug-cards-rage-4.0.0.tar.gz`. Die vorherige Fassung der Ressourcenprüfung liegt zusätzlich unter `backup/debug-cards-rage-4.0.0-checker.py`.

## Änderung 4.0.0: Debug-Kartenleiste, manuelle Zombies und Augen-Zombie-Grafik

- Die Debug-Pflanzenleiste wird nach dem nativen Start ausdrücklich sichtbar geschaltet und als geändert veröffentlicht. Die ausgewählten Pflanzen bleiben in der nativen Kartenleiste; der vorhandene Datenmodellweg aktualisiert Sichtbarkeit und Einträge.
- Im Debug-Level sind automatische Wellen-, Grab-, Wasser- und Himmelsspawns gesperrt. Zombiekarten verwenden weiterhin die native Erzeugung. Normale Level behalten ihre Spawns. Spezialfähigkeiten bereits platzierter Zombies bleiben Teil ihres nativen Verhaltens.
- Der Scared-Animationsadapter verwendet jetzt die aus dem Originalshader belegte Winkelmatrix. Die vorherigen Achsen und Vorzeichen verdrehten Körperteile. Die gemeinsam verwendete Korrektur gilt auch für die Blutsonnenblume.
- Der Augen-Zombie erhält Größe und Bodenanker vom tatsächlichen nativen Schatten statt vom Skinursprung. Der native Doppelschatten wird verborgen und bei Freigabe wiederhergestellt. Der importierte Körper und sein Schatten verwenden dieselbe Hierarchie und denselben Bodenpunkt; native Skinanimationen verändern diese Hierarchie nicht.

Die geänderte Debug-Spawnregel erhöht gemäß Versionsschema die erste Stelle auf **4.0.0**. Keine neuen Pflanzen, Zombies oder Rezepte; `fusionen.txt` bleibt unverändert. [Quellenbelege und manuelle Prüfliste](references/debug-roster-eyes-4-0-0/report.md). Spiel und Testprogramme wurden nicht gestartet. Rückbau bei geschlossenem Spiel: `backup/FusionMod-3.0.3.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/debug-roster-eyes-3.0.3.tar.gz`.

## Änderung 3.0.3: Fusionsergebnisse und bedienbare Debug-Auswahl

- Rechtsklick auf eine native Almanachpflanze zeigt jetzt die **erreichbaren Fusionsergebnisse** statt der Zutatenpartner. Jedes Ergebnis erscheint einmal mit seinem Namen und Bild. Seine Detailansicht nennt die passenden Zutaten und Rezepte für die angeklickte Pflanze. Alternative Rezepte werden unter demselben Ergebnis gesammelt.
- Der Debug-Dialog und seine Zombiekarten verwenden native Unity-UI-Knöpfe mit dem bestehenden EventSystem statt IMGUI. Backgroundklick öffnet die Kartenwahl; dort lassen sich bis zu zehn Pflanzen und im separaten Tab Zombiekarten wählen. Erst „3. Debug-Level starten“ startet das Level mit der gewählten Umgebung. Alle nativen Pflanzen sind unabhängig von Profilfreischaltungen wählbar. Imitater und die nur für Debug geltenden Freischaltungen verwenden weiterhin den vorhandenen Sitzungsweg.
- Backgroundwahl, Startanforderung und fertiges Debug-Board schreiben je einen Logeintrag. Fehlgeschlagene Aktionen zeigen ihre Fehlermeldung im Dialog. Die Levelkopie, der native Startweg und die vorhandenen Speichersperren bleiben zuständig.

Fix-Version **3.0.3**. Keine neuen Pflanzen, Zombies oder Rezepte; `fusionen.txt` bleibt unverändert. [Ursachen, Grenzen und manuelle Prüfliste](references/almanac-results-debug-ui-3-0-3/report.md). Spiel und Testprogramme wurden nicht gestartet. Vorherige DLL: `backup/FusionMod-3.0.2.dll`; vorheriger Projektstand: `backup/almanac-results-debug-ui-3.0.2.tar.gz`.

## Änderung 3.0.2: Backgroundauswahl im Debug-Dialog

- Die Backgroundauswahl verwendet zehn beschriftete Auswahlknöpfe pro Seite sowie Vor-/Zurückblättern und eine Mengenanzeige. `GUI.BeginScrollView` ist in dieser Replanted-Version nicht verfügbar und brach die Darstellung direkt nach „Background wählen“ ab. Der Dialog verwendet nun ausschließlich die bereits für die Kartenwahl verwendeten Knöpfe und Beschriftungen.
- Nach der Backgroundwahl beginnt die Kartenwahl wieder auf Seite eins. Eine leere Backgroundliste zeigt eine ausdrückliche Meldung. Die Auswahl bietet Textknöpfe, keine Bildvorschauen.

Fix-Version **3.0.2**, keine neuen Pflanzen, Zombies oder Gameplayregeln. [Ursache und manuelle Prüfliste](references/debug-background-picker-3-0-2/report.md). Spiel und Testprogramme bleiben ungestartet. Vorherige DLL: `backup/FusionMod-3.0.1.dll`; Projektdateien: `backup/debug-background-picker-3.0.1.tar.gz`.

## Änderung 3.0.1: Debug-Einstieg, freie Teilplätze und Almanachbilder

- `debug` im Hauptmenü verwendet jetzt den vorhandenen Gameplay-Chooser nach Dienstinjektion aus dem aktiven Hauptmenü. Der bislang stumme Abbruch wegen fehlender Dienste entfällt, ohne den normalen Spielstand-Ladeweg zu betreten.
- Kleine Pflanzen wechseln ohne passende Fusion auf den ersten freien Teilplatz desselben Feldes, wenn der angeklickte Teilplatz belegt ist. Ein freier angeklickter Teilplatz bleibt bevorzugt. Karten, Handschuh und Vorschau verwenden dieselbe Auswahl. Passende Fusionen haben Vorrang; volle Felder, Terrain und nicht kleine Pflanzen behalten ihre Sperren.
- Native Almanachpartner verwenden ihre Pflanzenminiatur statt des Rasenhintergrunds. Asynchron geladene Miniaturen aktualisieren Karten und Detailansicht automatisch; die nativen Modelle behalten die Ressourcenverwaltung.

Die letzte Versionsstelle steigt für diese Korrekturen auf **3.0.1**. Keine neuen Pflanzen, Zombies oder Rezepte; `fusionen.txt` bleibt unverändert. [Ursachen und manuelle Prüfliste](references/menu-slots-almanac-3-0-1/report.md). Spiel und Testprogramme wurden nicht gestartet. Statische Prüfung und Kompilierung ersetzen keine Laufzeitprüfung. Rückbau bei geschlossenem Spiel: `backup/FusionMod-3.0.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/menu-slots-almanac-3.0.0.tar.gz`.

## Änderung 3.0.0: Debug-Level und Fehlerkorrekturen

- Im Hauptmenü `debug` tippen: Background auswählen, anschließend bis zu zehn Pflanzen und im zweiten Tab beliebig viele Zombiekarten auswählen. Alle nativen Pflanzen stehen unabhängig vom Profil zur Verfügung. Imitater kopiert die erste andere gewählte Pflanze, ersatzweise Peashooter.
- Im Debug-Level bleiben normale Pflanzenkarten in der nativen Kartenleiste. Ausgewählte Zombiekarten erscheinen unten. Eine Zombiekarte anklicken, danach ein Rasenfeld anklicken: Der Zombie entsteht dort als Gegner. Rechtsklick oder Esc bricht die Zombieplatzierung ab. Der Augen-Zombie ist ebenfalls auswählbar.
- Native Wellen wiederholen sich ohne Levelende. Sonne lässt sich unbegrenzt ausgeben; die Anzeige bleibt bei 9990 und zeigt zusätzlich `∞`. Zombies am linken Rand beenden das Debug-Level nicht. Zombieauswahl, Freischaltung und Sonnenregeln gelten nur für diese Sitzung. Debug-Spielstände werden weder geladen noch gespeichert oder als Ersatz für bestehende Spielstände gelöscht.
- Der Almanach liest Fusionspartner aus dem vollständigen nativen Pflanzenmodell statt nur aus den sichtbaren Karten. Dadurch fehlen Laterne, Goldblume und andere außerhalb des aktuellen Ausschnitts liegende Pflanzen nicht mehr.
- Der Augen-Zombie öffnet die Augen erst bei einem feindlichen Ziel innerhalb eines Feldes in derselben Reihe. Projektile lösen das Öffnen nicht mehr aus. Die Originalanimation folgt dem Board-Controller unabhängig von der nativen Skinanimation; Teamwechsel spiegeln die importierte Grafik.
- Die Laterne erhält die Fusion-Aufladezeit von 15 Sekunden. Der bereits passende native Nebelbereich von 5×7 ohne Ecken, 25 Sonne und die vorhandene additive Lichtstufe 1 im Bereich 3×3 bleiben zuständig.
- Mutpilzsporen halten vor jeder nativen Kollisionsprüfung 40 Pixel Flughöhe. Mutpilzrauch und Eisrauch verwenden ihre Originalsprites und die archivierten Emissions-, Geschwindigkeits- und Ausblendwerte statt des nativen Standardrauchs. Schadensregeln bleiben im bestehenden Fume-Schadensweg.

Die Änderung bestehender Gameplayregeln erhöht die erste Versionsstelle von 2.35.0 auf 3.0.0. Keine neuen Fusionen; `fusionen.txt` bleibt unverändert. [Belege und manuelle Prüfliste](references/debug-fixes-3-0-0/report.md). Es wurden ausschließlich Daten geprüft und Programme kompiliert. Spiel und Testprogramme wurden nicht gestartet. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.35.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorherige Projektdateien liegen in `backup/debug-fixes-2.35.0.tar.gz`.

## Änderung 2.35.0: Vier Tang- und Matschkürbisfusionen

- **Tangle Kelp + Squash = Strangle Kelp** ([Würgetang](references/strangle-kelp/port-notes.md)): drei Ertränkungen, je 600 zusätzlicher Flächenschaden, eine Sekunde Erholung zwischen Griffen.
- **Tangle Kelp + Threepeater = Kelp-spreader** ([Dreifachtang](references/kelp-spreader/port-notes.md)): drei Tanggeschosse alle 1,5 Sekunden, je 30 Schaden und Tangpunkte; behält einmaliges Ertränken.
- **Squash + Threepeater = Squash-spreader** ([Dreifachmatschkürbis](references/squash-spreader/port-notes.md)): drei hüpfende Mini-Matschkürbisse alle drei Sekunden; 40 Schaden bei Kontakt und Bodenlandung.
- **Spikeweed + Squash = Squashweed** ([Matschkürbisgras](references/squashweed/port-notes.md)): 20 schilddurchdringender Kontaktschaden alle 1,5 Sekunden, Rückstoß und Mini-Matschkürbis.

Alle vier Quellenrezepte verwenden verfügbare native Zutaten. Beide Kartenreihenfolgen und Handschuhrichtungen sind aktiv. Vier Originalpflanzenmodelle mit sämtlichen Clips, das Originalgeschoss, Originalporträts und vollständiger deutscher/englischer Almanach sind enthalten. Dünger heilt ohne Entwicklung und erhält verbrauchte Griffe und laufende Angriffe. Der veraltete englische Kelp-Schadenswert ist anhand von PlantData und Originalcode korrigiert.

[Quellen-, Zustands- und Prüfbericht](references/kelp-squash-2-35-0/report.md), [Adapter und Benutzerkommandos](references/kelp-squash-2-35-0/adapters.json). Spiel und Testprogramme bleiben geschlossen; Tests übernimmt der Benutzer. Workflow und Quellenarchiv bleiben unverändert. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.34.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/support-pots-2.34.0.tar.gz`.


## Änderung 2.34.0: Vier Boden- und Wasserfusionen

- **Spikeweed + Kernel-pult = Butterweed** ([Buttergras](references/butterweed/port-notes.md)): halbes Bewegungstempo bei Kontakt, eine Sekunde Butter alle drei Sekunden, kein Schaden und keine Reifenzerstörung.
- **Spikeweed + Melon-pult = Springweed** ([Sprunggras](references/springweed/port-notes.md)): 80 schilddurchdringender Schaden alle 1,5 Sekunden; Hochschleudern, schwächere Wirkung auf Riesen, sofortiger Imp-Tod und geerbter Fahrzeugkontakt.
- **Sea-shroom + Squash = Splash** ([Wasserkürbis](references/splash/port-notes.md)): aquatischer Matschkürbis mit 1800 Landeschaden, auch gegen untergetauchte Ziele.
- **Sea-shroom + Wall-nut = Sea-nut** ([Meeresnuss](references/sea-nut/port-notes.md)): 2000/4000/6000 LP bei 0/10/20 Sekunden; rollt ab 30 Sekunden unverwundbar und verursacht bei Kontakt alle 0,02 Sekunden 600 Schaden.

Alle vier vollständigen Quellenrezepte verwenden bereits verfügbare native Zutaten. Beide Kartenreihenfolgen und Handschuhrichtungen sind aktiv. Keine Ergebnisidentität existiert nativ. Vier Originalmodelle mit elf Clips, Originalporträts und vollständigem deutschem/englischem Almanach sind enthalten. Alle Fähigkeiten sind umgesetzt; die vier Quellen erzeugen keine eigenen Schadensprojektile. Dünger heilt, beschleunigt Butter- beziehungsweise Wachstumstimer auf höchstens 0,5 Sekunden und erhält laufende Angriffe, Sprünge und Rollzustand. Sea-mine bleibt wegen der Kollision mit dem bestehenden ungeordneten Land-mine-Rezept offen.

[Gemeinsamer Quellen-/Zustandsbericht](references/ground-fusions-2-34-0/report.md), [Adapter und Benutzerkommandos](references/ground-fusions-2-34-0/adapters.json). Spiel und Testprogramme bleiben geschlossen; der Benutzer übernimmt Tests. Workflow und Quellenarchiv bleiben unverändert. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.33.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/pot-fusions-2.33.0.tar.gz`.


## Änderung 2.33.0: Vier weitere Kaktusfusionen

- **Cactus + Starfruit = Starthorn** ([Sternenkaktus](references/starthorn/port-notes.md)): fünf zielsuchende Originalsterne gegen Luftziele auf dem gesamten Feld.
- **Cactus + Doom-shroom = Doom Cactus** ([Unheilskaktus](references/doom-cactus/port-notes.md)): verstrahlende Dornen mit drei Treffern und Spezialfächer aus 90 Originaldornen; Schadens- und Quetschschutz während der Spezialanimation.
- **Cactus + Plantern = Lumos Cactus** ([Lichtkaktus](references/lumos-cactus/port-notes.md)): Luftpriorität, doppelte Luftangriffe, Lichtdornen und unbegrenzt durchdringende Laser mit Lichtbonus.
- **Cactus + Spikeweed = Thornweed** ([Kaktusstachelgras](references/thornweed/port-notes.md)): schilddurchdringender Kontakt gegen Boden und Luft; jeder Angriff schießt drei Originaldornen nach oben.

Alle vier vollständigen Quellenrezepte verwenden bereits verfügbare native Zutaten; beide Kartenreihenfolgen und Handschuhrichtungen bleiben unterstützt. Keine Ergebnisidentität existiert nativ. Alle Fähigkeiten und Projektilwege, Originalmodelle mit sämtlichen Clips, Originalporträts und vollständiger deutscher/englischer Almanach sind enthalten. Dünger heilt ohne Entwicklung und erhält Losvorrat und Angriff. Quellenwidersprüche stehen ausdrücklich im Almanach: Unheilskaktus startet mit 1/21 Spezialchance, Lichtstufe 3 verdoppelt Laserschaden.

[Gemeinsamer Bericht](references/cactus-fusions-2-33-0/report.md), [Adapter und Benutzerkommandos](references/cactus-fusions-2-33-0/adapters.json), [Installationsbeleg](references/release-2.33.0.json). Spiel und Testprogramme bleiben geschlossen; der Benutzer übernimmt Tests. Workflow und Quellenarchiv bleiben unverändert. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.32.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/cactus-fusions-2.32.0.tar.gz`.


## Änderung 2.32.0: Vier Laserschützen

- **Peashooter + Plantern = Beamshooter** ([Lichterbse](references/beamshooter/port-notes.md)): ein dauerhaft erfasstes Bodenziel.
- **Repeater + Plantern = Rebeamer** ([Doppellichterbse](references/rebeamer/port-notes.md)): zwei verschiedene Ziele.
- **Split Pea + Plantern = Split Beam** ([Relaislichterbse](references/split-beam/port-notes.md)): empfängt Energie von links; der rechte Kopf greift ein Ziel an.
- **Gatling Pea + Plantern = Gatling Beam** ([MG-Lichterbse](references/gatling-beam/port-notes.md)): vier verschiedene Ziele.

Alle vier Grundrezepte verwenden bereits verfügbare native Zutaten. Insgesamt sind alle zehn Quellenrezepte einschließlich der alternativen Erbsen-Aufwertungen implementiert, jeweils in beiden Kartenreihenfolgen und Handschuhrichtungen. Keine Ergebnisidentität existiert nativ. Jeder Strahl verursacht alle 0,2 Sekunden 10 schilddurchdringenden Grundschaden. Licht erzeugt Energie; Verbrauch verstärkt den Schaden. Vorne stehende Relaislichterbsen können stattdessen alle 0,1 Sekunden Energie empfangen, ab zwei Spalten Abstand. Ein Klick erfasst neue Ziele. Energie 1 bleibt gemäß Originalcode erhalten; eine Übertragung verbraucht einmal Energie, danach vervielfacht die Strahlzahl die übertragene Menge.

Vier Originalmodelle mit zwölf Clips, Originalporträts und vollständiger deutscher/englischer Almanach sind enthalten. Laser sind prozedural und erzeugen keine bewegten Schadensprojektile. Native Speicherfelder erhalten Energie, alle vier Zielkennungen, Relais, unabhängige Angriffstimer und Animation. Dünger heilt ohne besondere Entwicklung; Energie und Ziele bleiben erhalten. Nicht vorhandene Odyssee-/Rogue-Aufwertungen sind im Almanach abgegrenzt.

[Gemeinsamer Quellen-/Zustandsbericht](references/beam-fusions-2-32-0/report.md), [Adapter und Benutzerkommandos](references/beam-fusions-2-32-0/adapters.json), [Installationsbeleg](references/release-2.32.0.json). Spiel und Testprogramme bleiben geschlossen; der Benutzer übernimmt Tests. Workflow und `fusionen-quellen.json` bleiben unverändert. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.31.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/beam-fusions-2.31.0.tar.gz`.



## Änderung 2.31.0: Vier weitere Rauchfamilienfusionen

Alle vier Rezepte verwenden den bereits nativen **Fume-shroom**:

- **Plantern = Ray-shroom** ([Laserpilz](references/ray-shroom/port-notes.md)): unbegrenzt durchdringender Laser mit Distanz- und Lichtbonus, alle 1,5 Sekunden.
- **Kernel-pult = Blubber-shroom** ([Butterdrückpilz](references/blubber-shroom/port-notes.md)): Klick oder Treffer wirft Originalbutter auf nahe Ziele; 40 Schaden, vier Sekunden Betäubung, 30 aktuelle und maximale LP pro Angriff.
- **Sunflower = Sunrise-shroom** ([Sonnenspeicherpilz](references/sunrise-shroom/port-notes.md)): zieht neue Sonnen im 5×5-Bereich ein, speichert ihren abgerundeten 1,3-fachen Wert und zahlt bei der Produktion in 25er-Einheiten aus.
- **Umbrella Leaf = Bush-shroom** ([Schirmrauchpilz](references/bush-shroom/port-notes.md)): Schadensreduktion, gespeicherte Energie für Rauchwellen, Rückstoß sowie nativer Bungee-/Basketballschutz.

Alle Zutaten sind verfügbar; jedes Ergebnis besitzt genau dieses vollständige Quellenrezept. Keine Ergebnisidentität existiert nativ in Replanted. Originalmodelle enthalten acht Clips; das vollständige Originalbuttermodell wird wiederverwendet. Der Laser ist wie seine Quelle prozedural. Vollständiger deutscher/englischer Almanach mit Originalporträts ist enthalten. Dünger heilt alle vier; er erhält Butterdrückpilzes verringertes LP-Maximum sowie Energie und Sonnenspeicher. Beim Sonnenspeicherpilz begrenzt er den positiven Produktionstimer auf 0,5 Sekunden. Der Quellcode zahlt zusätzliche Sonne erst bei Speicher über 25 aus; diese Besonderheit ist umgesetzt und beschrieben.

[Quellen- und Zustandsbericht](references/fume-support-2-31-0/report.md), [Adapter und Benutzerkommandos](references/fume-support-2-31-0/adapters.json), [Installationsbeleg](references/release-2.31.0.json). Spiel und Testprogramme bleiben geschlossen; der Benutzer übernimmt Tests. Workflow und `fusionen-quellen.json` bleiben unverändert. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.30.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/fume-support-2.30.0.tar.gz`.


## Änderung 2.30.0: Vier weitere Fusionen

- **Garlic + Umbrella Leaf = Umbrella Clove** ([Knoblauchschirm](references/umbrella-clove/port-notes.md)): 40 schilddurchdringender Schaden, Rückstoß und nativer Reihenwechsel.
- **Cactus + Umbrella Leaf = Umbrella Thorn** ([Dornenschirm](references/umbrella-thorn/port-notes.md)): stößt ausschließlich Luftziele zurück und zerplatzt Ballons ohne direkten Schaden.
- **Cabbage-pult + Doom-shroom = Damage-pult** ([Unheilkohlapult](references/damage-pult/port-notes.md)): 1800 Schaden alle 15 Sekunden; verstrahlte Treffer lösen 600 Flächenschaden gegen andere nahe Zombies aus.
- **Fume-shroom + Starfruit = Starburst-shroom** ([Sternkappenpilz](references/starburst-shroom/port-notes.md)): schadensfreie Kappe mit drei verschiedenen Kontakten; jeder Kontakt erzeugt fünf Originalsterne mit je 20 Schaden.

Alle Zutaten waren bereits nativ verfügbar. Keine Ergebnisidentität existiert nativ in Replanted. Alle vier vollständigen Quellenrezepte sind in beiden Kartenreihenfolgen und Handschuhrichtungen aktiv. Knoblauch- und Dornenschirm besitzen 1000 LP, eine Sekunde Kontaktabklingzeit und die ursprünglichen eigenen Gesundheitskosten von 100/150/250 LP für 1/2–4/5+ Ziele. Nativer Schutz gegen Bungees und Katapultgeschosse bleibt erhalten. Dünger heilt alle vier ohne Sonderentwicklung oder Rücksetzen der Angriffszähler. Originale Luftschiffe sind in Replanted nicht vorhanden; diese Grenze ist im Almanach benannt.

Vier Originalpflanzenmodelle und zwei zusätzliche vollständige Projektilmodelle mit insgesamt elf Clips sowie das vorhandene Originalmodell normaler Sterne sind enthalten. Der Almanach besitzt Originalporträts sowie vollständige deutsche/englische Beschreibungen und Begleittexte. Aktiv sind **309 Rezepte**. Offen bleiben **360 Rezepte mit 294 Ergebnissen**. Der Almanach enthält **258 Ergebnisse**. Die vier vollständigen Zeilen entfallen aus `fusionen.txt`; Quellenarchiv und Workflow bleiben unverändert.

[Gemeinsamer Quellen-/Zustandsbericht](references/mixed-fusions-2-30-0/report.md), [Adapter und Benutzerkommandos](references/mixed-fusions-2-30-0/adapters.json), [Installationsbeleg](references/release-2.30.0.json). Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.29.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/umbrella-fusions-2.29.0.tar.gz`.


## Änderung 2.29.0: Vier Unheilschützen

- **Peashooter + Doom-shroom = Doom Pea** ([Verderbse](references/doom-pea/port-notes.md)): Schild durchdringende Erbsen. Jeder Schuss erhöht den Schaden um eins bis 100; der erste verursacht bereits 41 Schaden.
- **Gatling Pea + Doom-shroom = Gatling Doom** ([MG-Unheil](references/gatling-doom/port-notes.md)): vier Geschosse zu je 300 Schaden. Jedes sechzehnte verursacht 1800 Direkt- und zusätzlichen Flächenschaden; anschließend 4,5 Sekunden Abklingzeit. Alternativrezept: **Doom Pea + Split Pea**.
- **Threepeater + Doom-shroom = Doom-spreader** ([Unheildreifacherbse](references/doom-spreader/port-notes.md)): drei Erbsen zu je 40 Schaden. Jede siebte Salve verwendet große Geschosse mit 120 Direkt- und Flächenschaden. Am Rand ersetzt ein verzögertes Zusatzgeschoss die fehlende Reihe.
- **Starfruit + Doom-shroom = Death Star** ([Todesstern](references/death-star/port-notes.md)): nach drei Sekunden ein fallender Unheilmeteor mit 1800 Flächenschaden und 180 Originalsporensternen zu je 20 Schaden. Kein Krater.

Alle vier Ergebnisse besitzen verfügbare native Grundrezepte. Insgesamt sind fünf neue Rezepte aktiv, jeweils in beiden Kartenreihenfolgen und Handschuhrichtungen. Keine Variante existiert nativ in Replanted. Gatling Frenzy-shroom als weitere Zutat bleibt offen, weil diese Pflanze noch nicht implementiert ist. Doom-spreader erhält ein eigenes Speicherpaar; Charred Threepeater behält seine bestehende Kennung.

Dünger heilt alle vier und begrenzt Death Stars positiven Aktivierungstimer auf 0,5 Sekunden. Schadensbonus, Geschoss-/Salvenzähler und Pause bleiben erhalten. Große Unheilgeschosse führen laut Quellcode Flächenschaden vor dem Direkttreffer aus. Drei Quelleinheiten Radius entsprechen etwa 2,22 Rasenfeldern. Nicht vorhandene Frenzy-Synergien und Odysseeaufwertungen sind im Almanach ausdrücklich abgegrenzt.

Vier Originalpflanzenmodelle und vier zusätzliche vollständige Geschossmodelle mit insgesamt 19 Clips sowie das bestehende Originalmodell der Sporensterne sind enthalten. Englischer und deutscher Almanach besitzen vollständige Texte und Originalporträts. Aktiv sind **305 Rezepte**. Offen bleiben **364 Rezepte mit 298 Ergebnissen**. Der Almanach enthält **254 Ergebnisse**. Die fünf vollständigen Rezeptzeilen entfallen aus `fusionen.txt`; Quellenarchiv und Workflow bleiben unverändert.

[Gemeinsamer Quellen-/Zustandsbericht](references/doom-shooters-2-29-0/report.md), [Adapter und Benutzerkommandos](references/doom-shooters-2-29-0/adapters.json), [Installationsbeleg](references/release-2.29.0.json). Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.28.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/support-pots-2.28.0.tar.gz`.

## Änderung 2.28.0: Vier weitere Metallfusionen

- **Kernel-pult + Magnet-shroom = Kernel Morph** ([Magnetkornapult](references/kernel-morph/port-notes.md)): 60 Schaden alle drei Sekunden; zu 25 Prozent magnetische Butter mit 80 Schaden und vier Sekunden Betäubung.
- **Morph-shroom + Bucket = Rust-shroom** ([Rostpilz](references/rust-shroom/port-notes.md)): alle 1,5 Sekunden 50 schilddurchdringender Schaden, 150 gegen verbleibende Typ-1-Rüstung.
- **Melon Morph + Bucket = Metal-pult** ([Eisenmelonapult](references/metal-pult/port-notes.md)): 200 Direktschaden plus Melonenspritzschaden alle drei Sekunden; doppelter Schaden gegen vorhandene Typ-1-Rüstung jedes Empfängers.
- **Starmorph + Bucket = Iron Star** ([Eisenstern](references/iron-star/port-notes.md)): fünf Sterne mit je 80 Schaden alle 1,5 Sekunden; addiert vorhandene Typ-2-Rüstung vor der nativen Schadensverarbeitung.

Alle Zutaten waren bereits verfügbar. Jedes Ergebnis besitzt genau ein vollständiges Quellenrezept; beide Reihenfolgen und Handschuhrichtungen verwenden den vorhandenen Resolver. Aufnehmbare Eimer können die drei Metallgrundpflanzen aufwerten und danach jeweils 500 LP bis zum Maximum von 2000 heilen. Kernel Morph besitzt 300 LP. Dünger heilt alle vier ohne Sonderentwicklung. Nicht implementierte Chrono-/Helmaufwertungen bleiben offen. Keine Variante existiert nativ in Replanted.

Vier Originalpflanzenmodelle und vier vollständige Originalprojektilmodelle mit insgesamt 13 Clips sowie Originalporträts und vollständiger deutscher/englischer Almanach sind enthalten. Die Rauchfamilie verwendet ihren bestehenden nativen FumeCloud-Effekt. Aktiv sind **300 Rezepte**. Offen bleiben **369 Rezepte mit 301 Ergebnissen**. Der Almanach enthält **250 Ergebnisse**. Die vier vollständigen Zeilen entfallen aus `fusionen.txt`; Quellenarchiv und Workflow bleiben unverändert.

[Gemeinsamer Quellen-/Zustandsbericht](references/shooter-fusions-2-28-0/report.md), [Adapter und Benutzerkommandos](references/shooter-fusions-2-28-0/adapters.json), [Installationsbeleg](references/release-2.28.0.json). Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.27.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/shooter-fusions-2.27.0.tar.gz`.


## Änderung 2.27.0: Vier unterstützende Kürbisfusionen

Alle vier Rezepte verwenden **Pumpkin** und die genannte, bereits native Zutat:

- **Peashooter = Pumpkin Pod** ([Erbsenkürbis](references/pumpkin-pod/port-notes.md)): kopiert Projektiltyp, Schaden und Salven des eingeschlossenen Erbsenschützen. Doppeltes Grundintervall, mindestens drei Sekunden.
- **Melon-pult = Melon Shell** ([Melonenhülle](references/melon-shell/port-notes.md)): pro tatsächlichem Wurf eine zusätzliche Originalmelone mit 80 Direktschaden und Melonenspritzschaden.
- **Ice-shroom = Cryo Pumpkin** ([Frostkürbis](references/cryo-pumpkin/port-notes.md)): speichert bis zu drei Eiswirkungen vom Feld direkt davor; Klick und automatische Freisetzung mit vier Sekunden Gefrieren. Bisse verursachen vier Sekunden Kälte und fünf Gefrierpunkte.
- **Jalapeno = Pyro Pumpkin** ([Glutkürbis](references/pyro-pumpkin/port-notes.md)): Wärme aus Feuerzeilen und überhitzten Todesfällen, Bissreflexion und Überhitzung. Klick oder Tod verbrauchen ab 1800 Wärme sämtliche Energie für eine Feuerzeile.

Jedes Ergebnis besitzt ein vollständig verfügbares Quellenrezept; beide Kartenreihenfolgen und Handschuhrichtungen verwenden den vorhandenen Resolver. Keine Variante existiert nativ. Alle Hüllen besitzen 4000 Lebenspunkte. Vier Originalmodelle mit insgesamt sechs Clips und vorhandene vollständige Projektilmodelle sind enthalten. Dünger heilt; beim Erbsen- und Frostkürbis begrenzt er positive Fähigkeitstimer auf 0,5 Sekunden. Ladungen und Wärme bleiben erhalten. Überhitzung wird für echtes Speichern/Laden in nativen GridItems gespeichert. Englische und deutsche Almanachtexte benennen belegte Abweichungen: Cryo erreicht das eigene Feld entgegen der Beschreibung nicht, Pumpkin Pod hat im Quellcode keine sechsekündige Obergrenze, beschädigte Pyro-Hüllen reflektieren weniger Schaden.

Aktiv sind **296 Rezepte**. Offen bleiben **373 Rezepte mit 305 Ergebnissen**; die vier vollständigen Zeilen entfallen aus `fusionen.txt`. Der Almanach enthält **246 Ergebnisse**. Workflow und Quellenarchiv bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests.

[Gemeinsamer Quellen-/Zustandsbericht](references/pumpkin-support-2-27-0/report.md), [Adapter und Benutzerkommandos](references/pumpkin-support-2-27-0/adapters.json), [Installationsbeleg](references/release-2.27.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.26.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/pumpkin-support-2.26.0.tar.gz`.


## Änderung 2.26.0: Vier Unheilfusionen

Alle vier Rezepte verbinden den bereits nativen **Doom-shroom** mit einer ebenfalls nativen Zutat:

- **Garlic = Doom Garlic** ([Unheilknoblauch](references/doom-garlic/port-notes.md)): Bisse verstrahlen und vergiften; einmalige Leichenexplosion mit 300 Schaden. Gift bleibt bei Verstrahlung aktiv.
- **Wall-nut = Doom-nut** ([Unheilnuss](references/doom-nut/port-notes.md)): 4000 LP, Schutz gegen nichtphysische Explosionen, Heilung bei nahen verstrahlten Todesfällen und Unheilpilzeffekt beim eigenen Tod.
- **Sunflower = Doomflower** ([Unheilsonnenblume](references/doomflower/port-notes.md)): alle 50 Sekunden 25 Sonne und eine Unheilspore in 3×3. Sporen wachsen 30 Sekunden und explodieren bei Gegnernähe.
- **Potato Mine = Crater-tot** ([Kraterkartoffel](references/crater-tot/port-notes.md)): Kartoffel- und Unheilexplosion, anschließend fünf Originalfragmente. Nach 1,5 Sekunden Flug entstehen native Minen mit 0,5 Sekunden Restzeit vor dem Auftauchen.

Jedes Ergebnis besitzt genau ein Quellenrezept, gültig für beide Kartenreihenfolgen und Handschuhrichtungen. Keine Variante existiert nativ. Fünf Originalpflanzenmodelle einschließlich der erzeugten Spore und das vollständige Fragmentmodell mit insgesamt zehn Clips sind enthalten. Der Almanach enthält vollständige deutsche/englische Texte und Originalporträts. Dünger heilt; Sonnen-/Sporentimer werden auf höchstens 0,5 Sekunden begrenzt, Crater-tot wird sofort scharf. Replanted besitzt keine Doom-shroom Zombies; deren getrennte Nussunterdrückung hat keinen Zieltyp.

Aktiv sind **292 Rezepte**. Offen bleiben **377 Rezepte mit 309 Ergebnissen**; die vier vollständigen Zeilen entfallen aus `fusionen.txt`. Der Almanach enthält **242 Ergebnisse**. Workflow und Quellenarchiv bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests.

[Gemeinsamer Quellen-/Zustandsbericht](references/doom-fusions-2-26-0/report.md), [Adapter und Benutzerkommandos](references/doom-fusions-2-26-0/adapters.json), [Installationsbeleg](references/release-2.26.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.25.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/support-pots-2.25.0.tar.gz`.


## Änderung 2.25.0: Drei Kakteen und eine Leuchtnuss

- **Fume-shroom + Cactus = Saguaro-shroom** ([Säulenkakteenpilz](references/saguaro-shroom/port-notes.md)): schilddurchdringender Rauch, niedriger Boden-/Luftmodus und hoher Luftmodus mit vierfachem Schaden. Klick wechselt den Modus.
- **Cactus + Ice-shroom = Ice Cactus** ([Eiskaktee](references/ice-cactus/port-notes.md)): Luftpriorität, zehn Frostpunkte, vierfacher Schaden gegen gefrorene Ziele sowie zusätzlich gegen Luft; bis zu drei verschiedene Lufttreffer.
- **Cactus + Magnet-shroom = Magnet Cactus** ([Magnetkaktee](references/magnet-cactus/port-notes.md)): Luftpriorität, 40 Grundschaden, doppelter Schaden gegen vorhandene Metallrüstung/-schilde.
- **Plantern + Wall-nut = Illumi-nut** ([Leuchtnuss](references/illumi-nut/port-notes.md)): 4000 LP, Lichtstufe eins in 3×3, alle 15 Sekunden Heilung aus zusätzlichem Licht.

Jedes Ergebnis besitzt genau ein vollständig verfügbares Quellenrezept. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden den vorhandenen Resolver. Keine Variante existiert nativ. Vier Originalpflanzenmodelle mit sämtlichen 19 Clips sowie beide vollständigen Originalprojektilmodelle sind enthalten. Der Almanach erhält Originalporträts und vollständige deutsche/englische Texte. Dünger heilt alle vier; ausschließlich der positive Illumi-Heilungstimer wird auf höchstens 0,5 Sekunden begrenzt. Nicht vorhandene Eisen-/Portalballons und Luftschiffe bieten den zusätzlichen Magnet-Quellenzweigen keinen Zieltyp.

Aktiv sind **288 Rezepte**. Offen bleiben **381 Rezepte mit 313 Ergebnissen**; vier vollständige Zeilen entfallen aus `fusionen.txt`. Der Almanach enthält **238 Ergebnisse**. Workflow und Quellenarchiv bleiben unverändert. Spiel und Testprogramme bleiben geschlossen; der Benutzer übernimmt Tests.

[Gemeinsamer Quellen-/Zustandsbericht](references/cactus-fusions-2-25-0/report.md), [Adapter und Benutzerkommandos](references/cactus-fusions-2-25-0/adapters.json), [Installationsbeleg](references/release-2.25.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.24.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/cactus-fusions-2.24.0.tar.gz`.


## Änderung 2.24.0: Vier Metallfusionen

Alle Rezepte verwenden **Magnet-shroom** sowie die genannte, bereits native Zutat:

- **Doom-shroom = Dormant-shroom** ([Schlummerpilz](references/dormant-shroom/port-notes.md)): bleibt passiv und explodiert nicht.
- **Fume-shroom = Morph-shroom** ([Metallrauchpilz](references/morph-shroom/port-notes.md)): 40 Schild durchdringender Schaden alle 1,5 Sekunden; doppelt gegen Metallrüstung/-schilde und Zamboni.
- **Melon-pult = Melon Morph** ([Metallmelone](references/melon-morph/port-notes.md)): 120 Direktschaden alle drei Sekunden mit Melonenspritzschaden; doppelter Schaden bei vorhandener Typ-1-Rüstung jedes Empfängers.
- **Starfruit = Starmorph** ([Metallstern](references/starmorph/port-notes.md)): fünf Sterne mit je 40 Schaden alle 1,5 Sekunden; doppelter Schaden gegen Metallrüstung/-schilde.

Jedes Ergebnis besitzt genau ein Quellenrezept, gültig für beide Kartenreihenfolgen und Handschuhrichtungen. Vier Originalpflanzenmodelle und zwei vollständige Projektilmodelle samt Clips, Originalporträts und vollständigen deutschen/englischen Almanachtexten sind enthalten. Dünger heilt ohne Sonderentwicklung. Spätere Metallupgrades bleiben offen; sie sind andere Ergebnisse. Keine neue Variante existiert nativ in Replanted.

Aktiv sind **284 Rezepte**. Offen bleiben **385 Rezepte mit 317 Ergebnissen**; vier vollständige Zeilen entfallen aus `fusionen.txt`. Der Almanach enthält **234 Ergebnisse**. Workflow und Quellenarchiv bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests.

[Gemeinsamer Quellen-/Zustandsbericht](references/metal-morphs-2-24-0/report.md), [Adapter und Benutzerkommandos](references/metal-morphs-2-24-0/adapters.json), [Installationsbeleg](references/release-2.24.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.23.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorheriger Projektstand: `backup/support-pots-2.23.0.tar.gz`.


## Änderung 2.23.0: Vier neue Wurffusionen

- **Cabbage-pult + Squash = Lettuce-pult** ([Salatquetschapult](references/lettuce-pult/port-notes.md)): Quetschgeschosse mit zwei Folgegenerationen, 60/30/15 Schaden.
- **Kernel-pult + Squash = Butternut-pult** ([Butterquetschapult](references/butternut-pult/port-notes.md)): 60 Quetschschaden; 25 Prozent Butterauswahl, vier Sekunden Butter für bis zu vier Ziele gemäß tatsächlichem Quellcode.
- **Melon-pult + Squash = Gourd-pult** ([Melonenquetschapult](references/gourd-pult/port-notes.md)): 120 Quetschschaden; nach einer Sekunde zusätzlicher Melonenspritzschaden.
- **Fume-shroom + Melon-pult = Spring Melon** ([Federmelone](references/spring-melon/port-notes.md)): gespeicherte Ladung bis 30 Sekunden, bis 1920 Direktschaden. Quellenbelegt: drei Sekunden Grundintervall, sieben Quelleinheiten Reichweite und mehr als 1,5 Sekunden erforderliche Ladung. Diese Codewerte ersetzen widersprüchliche englische Almanachangaben.

Jedes Ergebnis besitzt genau ein Quellenrezept. Alle Zutaten waren bereits nativ verfügbar; keine neue Variante existiert nativ. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die bestehende Auflösung. Vier Originalpflanzenmodelle und fünf neue Projektil-/Bodenmodelle mit sämtlichen Clips sind enthalten; Spring Melon verwendet das vorhandene vollständige Melonenprojektil. Alle vier erhalten englische und deutsche Almanachtexte samt Originalporträts. Dünger heilt; bei Spring Melon begrenzt er ausschließlich den positiven Attributtimer auf 0,5 Sekunden und erhält die Ladung.

Aktiv sind **280 Rezepte**. Offen bleiben **389 Rezepte mit 321 Ergebnissen**; vier vollständig implementierte Zeilen entfallen aus `fusionen.txt`. Der Almanach enthält **230 Ergebnisse**. Workflow und Quellenarchiv bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests.

[Gemeinsamer Quellen-/Zustandsbericht](references/squash-lobbers-2-23-0/report.md), [Adapter und Benutzerkommandos](references/squash-lobbers-2-23-0/adapters.json), [Installationsbeleg](references/release-2.23.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.22.0.dll` nach `Mods/FusionMod.dll` kopieren. Projektstand davor: `backup/squash-lobbers-2.22.0.tar.gz`.

## Änderung 2.22.0: Vier Kürbisfusionen und Fehlerkorrekturen

- **Pumpkin + Blover = Windy Pumpkin** ([Windkürbis](references/windy-pumpkin/port-notes.md)): 4000 LP; erzeugt alle 30 Sekunden einen nativen Blover im ersten zulässigen freien Innenfeld ab dem eigenen Feld nach rechts.
- **Pumpkin + Spikeweed = Spiky Pumpkin** ([Stachelkürbis](references/spiky-pumpkin/port-notes.md)): 4000 LP; niedrig, blockiert keine Zombies. Übernimmt Quetschen für sich und Stachelpflanzen/Stachelnüsse im selben Feld. Reifenverlust kostet 500 LP, höchstens einmal pro Simulationstick.
- **Pumpkin + Plantern = Jack O' Plantern** ([Laternenkürbis](references/jack-o-plantern/port-notes.md)): 4000 LP; Lichtstufe 1 und Nebelklärung in 3×3. Alle 15 Sekunden Heilung um 150 × max(0, Lichtstufe im Feld − 1) LP. Implementierte Lichtquellen addieren sich.
- **Pumpkin + Sea-shroom = Sea-shell** ([Meerkürbis](references/sea-shell/port-notes.md)): 4000 LP; Meerpilzkarten, getragene Meerpilze und Schwarmmeerpilze unmittelbar links reparieren 1333 LP. Innenpflanze und Unterlage bleiben erhalten.

Alle vier Ergebnisse besitzen genau ein Quellenrezept, unterstützt in beiden Kartenreihenfolgen und Handschuhrichtungen. Keine Variante existiert nativ. Vier Originalmodelle mit sämtlichen vier Originalclips, Schadensansichten und vollständigen englischen/deutschen Almanachtexten samt Originalporträts sind enthalten. Diese vier Pflanzen besitzen keine eigenen Projektile; Windkürbis erzeugt einen vollständigen nativen Blover. Dünger heilt alle vier; beim Wind- und Laternenkürbis begrenzt er den laufenden Fähigkeitstimer auf höchstens 0,5 Sekunden.

Der Almanach löst beim Rechtsklick auf eine native Karte zuerst den vollständigen Binderschlüssel auf. Cattail- und Melonentopf-Patches berücksichtigen beide nativen `AddProjectile`-Überladungen ohne mehrdeutiges Harmony-Ziel. Chompzillas gespeicherte Fragmentkennung bestimmt ausdrücklich das genaue Originalmodell; eine andere gültige Geschossansicht genügt nicht. Beide nativen Erbsentypen erhalten die Fragmentkennung. Alle drei Originalfragmente sind erneut exportiert.

Aktiv sind **276 Rezepte**; offen bleiben **393 Rezepte mit 325 Ergebnissen**. Der Almanach enthält **226 Ergebnisse**. Vier vollständig erledigte Zeilen entfallen aus `fusionen.txt`. Quellenarchiv und Workflow bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests.

[Gemeinsamer Quellen-/Zustandsbericht](references/pumpkin-support-2-22-0/report.md), [Adapter und Benutzerkommandos](references/pumpkin-support-2-22-0/adapters.json), [Installationsbeleg](references/release-2.22.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.21.0.dll` nach `Mods/FusionMod.dll` kopieren und einen Spielstand ohne neue Kennungen laden. Projektstand davor: `backup/pumpkin-support-2.21.0.tar.gz`.



## Änderung 2.21.0: Vier Kürbisfusionen und Grafik-/Trefferkorrekturen

- **Pumpkin + Cherry Bomb = Cherry Pumpkin** ([Kirschkürbis](references/cherry-pumpkin/port-notes.md)): 4000 LP; absorbiert explosive Kirschen vor deren Flächenschaden. Absorption und vorhandene Kirsch-Flächeneffekte heilen 200 LP.
- **Pumpkin + Garlic = Stinky Pumpkin** ([Stinkkürbis](references/stinky-pumpkin/port-notes.md)): 4000 LP; tatsächliche Bisse geben zehn Sekunden Gift, einen Giftpunkt und den nativen verzögerten Reihenwechsel.
- **Pumpkin + Scaredy-shroom = Scareless Pumpkin** ([Mutkürbis](references/scareless-pumpkin/port-notes.md)): 2000 LP; native und implementierte Angstpilze im selben Feld bleiben mutig und schießen weiter.
- **Pumpkin + Wall-nut = Nut Shell** ([Nussschale](references/nut-shell/port-notes.md)): 8000 LP; etwa alle 50 Sekunden insgesamt 1000 LP Heilung für andere Kürbisse im umliegenden 3×3-Bereich. Ohne Heilziel entsteht eine verwendbare Wallnusskarte, höchstens dreimal. Heilung bleibt danach aktiv. Die Quelle lässt die Nuss von 0,01 auf 1 mit 0,02/s wachsen: ungefähr 49,5 Sekunden.

Alle vier Quellenrezepte gelten für beide Kartenreihenfolgen und Handschuhrichtungen. Innenpflanze und Unterlage bleiben erhalten. Vier Originalmodelle mit allen vier Clips, Schadensansichten, sichtbarem Nusswachstum und vollständigen deutschen/englischen Almanachtexten samt Originalporträts sind enthalten. Keine dieser Varianten existiert nativ in Replanted. Dünger heilt ohne Sonderentwicklung; Nusswachstum und verbrauchte Karten bleiben erhalten.

Chompzillas drei Originalfragmente werden erneut aus den Quellenprefabs exportiert. Grafikansichten prüfen jetzt die genaue Fragmentkennung und werden im vorhandenen Bewegungsweg wiederhergestellt. Alle Fume-Fusionen und Shivery-shroom verwenden den Quellenmaßstab statt uneinheitlicher Schattenbreiten. Selbst erzeugte Scaredy-Geschosse erhalten die erforderliche Boden-Zielmaske; ältere gespeicherte Schüsse werden vor der nächsten Kollision repariert. Frost-shrooms Rauch erhält das Blau des originalen FumeIce-Sprites.

Aktiv sind **272 Rezepte**; offen bleiben **397 Rezepte mit 329 Ergebnissen**. Der Almanach enthält **222 Ergebnisse**. Vier erledigte Zeilen entfallen aus `fusionen.txt`. Quellenarchiv und Workflow bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; der Benutzer übernimmt Tests.

[Quellen-, Zustands- und Prüfbericht](references/pumpkin-elemental-2-21-0/report.md), [Adapter und Benutzerkommandos](references/pumpkin-elemental-2-21-0/adapters.json), [Installationsbeleg](references/release-2.21.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.20.0.dll` nach `Mods/FusionMod.dll` kopieren. Projektstand davor: `backup/pumpkin-elemental-2.20.0.tar.gz`.



## Änderung 2.20.0: Vier Kürbisfusionen, Chompzilla und Boost-shroom

- **Pumpkin + Starfruit = Starry Pumpkin** ([Sternenkürbis](references/starry-pumpkin/port-notes.md)): ermöglicht automatische Solarfruit-Schüsse. Die Anziehungssynergien setzen die noch nicht implementierten Starblover/Pulsar voraus.
- **Pumpkin + Cactus = Thorny Pumpkin** ([Dornenkürbis](references/thorny-pumpkin/port-notes.md)): 4000 LP; reflektiert 50 Schaden bei jedem tatsächlich erlittenen Biss.
- **Pumpkin + Puff-shroom = Puff-kin** ([Sprösselkürbis](references/puff-kin/port-notes.md)): beginnt mit 1000/4000 LP, heilt 50 LP alle drei Sekunden. Oberhalb eines Drittels halbierter numerischer Schaden; oberhalb von zwei Dritteln Schutz der Innenpflanze.
- **Pumpkin + Fume-shroom = Fuming Pumpkin** ([Rauchkürbis](references/fuming-pumpkin/port-notes.md)): drei vertikale, schilddurchdringende 20-Schaden-Treffer alle 1,5 Sekunden, eigene Spalte bis drei Reihen darüber/darunter.

Alle vier Quellenrezepte gelten für beide Kartenreihenfolgen und Handschuhrichtungen. Kürbisauflösung erhält Innenpflanze und Unterlage. Vier Originalmodelle, alle fünf Clips, Schadensansichten sowie deutsche/englische Almanachtexte mit Originalporträts sind enthalten. Dünger heilt; beim Sprösselkürbis begrenzt er die verbleibende Heilungszeit auf höchstens 0,5 Sekunden.

Chompzilla verwendet jetzt alle drei ursprünglichen Zombiefragmente statt ChompShooter-Grafik. Ausspucken folgt dem Originalereignis nach dem Biss. Schwache ungepanzerte Ziele verbrauchen keinen Spezialbiss. Spezialverdauung dauert 40 Sekunden; Dünger begrenzt ihre Restzeit auf höchstens 0,5 Sekunden. Geschosskennung und Trefferhistorie bleiben beim Speichern erhalten. Boost-shrooms Rauch erhält Originalgrün und reicht über die vollständige vordere Reihe; funktionierender Schaden und Schub bleiben beim vorhandenen Besitzer.

Aktiv sind **268 Rezepte**, offen bleiben **401 Rezepte mit 333 Ergebnissen**. Der Almanach enthält **218 Ergebnisse**. Vier erledigte Zeilen entfallen aus `fusionen.txt`. Quellenarchiv und Workflow bleiben unverändert. Spiel und Testprogramme werden nicht gestartet; Spieltests übernimmt der Benutzer.

[Gemeinsamer Quellen-/Zustandsbericht](references/pumpkin-fusions-2-20-0/report.md), [Adapter und Benutzerkommandos](references/pumpkin-fusions-2-20-0/adapters.json), [Installationsbeleg](references/release-2.20.0.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.19.0.dll` nach `Mods/FusionMod.dll` kopieren. Der vorherige Projektstand liegt unter `backup/pumpkin-fusions-2.19.0.tar.gz`.


## Änderung 2.19.0: Vier Wasser-Unterstützungsfusionen

- **Sea-shroom + Plantern = Beacon-shroom** ([Leuchtmeerpilz](references/beacon-shroom/port-notes.md)): klärt 3×3 Felder auf; nach 30 Sekunden 5×5.
- **Sea-shroom + Hypno-shroom = Jelly-shroom** ([Quallenhypnopilz](references/jelly-shroom/port-notes.md)): Meerpilzfutter beschwört nach der Fressanimation einen hypnotisierten Schwimmreifenzombie. Bisse hypnotisieren den Angreifer und verbrauchen die wache Pflanze.
- **Sea-shroom + Ice-shroom = Swell-shroom** ([Frostkugelfischpilz](references/swell-shroom/port-notes.md)): Meerpilzfutter löst nach 1,4 Sekunden 20 schilddurchdringenden Schaden und vier Sekunden Frost im Nahbereich aus.
- **Sea-shroom + Scaredy-shroom = Hermit-shroom** ([Einsiedlermeerpilz](references/hermit-shroom/port-notes.md)): Meerpilzfutter gibt zehn Sekunden Mut, einschließlich Aufrichtzeit. Schießt 20-Schaden-Sporen alle 0,5 Sekunden; kauert danach wieder.

Alle vier Quellenrezepte gelten für beide Kartenreihenfolgen und Handschuhrichtungen. Fütterung funktioniert mit Karte, getragenem Meerpilz und Schwarmmeerpilz unmittelbar links. Vier Originalmodelle, alle 13 Originalclips, vorhandene Originalsporen und vollständige deutsche/englische Almanachtexte samt Porträts sind enthalten. Native Wasser-, Schlaf-, Kaffee- und Speicherregeln gelten. Dünger heilt und begrenzt verbleibendes Beacon-Wachstum beziehungsweise laufenden Hermit-Mut auf höchstens 0,5 Sekunden. Er aktiviert versteckte Hermits nicht und erhält Jelly-/Swell-Aktionsphasen.

Der Almanach enthält **214 Ergebnisse**. Aktiv sind **264 Rezepte**. Vier erledigte Einträge entfallen aus `fusionen.txt`; **405 Rezepte mit 337 Ergebnissen** bleiben offen. Bestehende Pflanzen werden nicht dupliziert. Workflow und Quellenarchiv bleiben unverändert.

[Gemeinsamer Quellen-/Zustandsbericht](references/sea-support-2-19-0/report.md), [Adapter und Benutzerkommandos](references/sea-support-2-19-0/adapters.json), [Installationsbeleg](references/release-2.19.0.json). Spiel und Testprogramme bleiben geschlossen. Der Benutzer prüft Fähigkeiten, Futterkosten, Mut-/Wachstumszeiten, Dünger, Wasserbewegung beschworener Zombies, Originalanimationen, Handschuh, Pause/Tod, echtes Speichern/Laden und beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.18.0.dll` nach `Mods/FusionMod.dll` kopieren. Danach einen Spielstand ohne neue Kennungen laden. Der ursprüngliche Projektstand liegt unter `backup/sea-support-2.18.0.tar.gz`.


## Änderung 2.18.0: Vier Wasserfusionen

Vier neue Ergebnisse verwenden ausschließlich zuvor verfügbare native Zutaten. Jedes besitzt genau ein Quellenrezept, registriert für beide Kartenreihenfolgen und Handschuhrichtungen. Keine Variante existiert bereits nativ.

- **Sea-shroom + Puff-shroom = Minnow-shroom** ([Schwarmmeerpilz](references/minnow-shroom/port-notes.md)): zwei 20-Schaden-Sporen alle 1,5 Sekunden; nach 15 Sekunden drei. Danach alle 15 Sekunden Anwendung eines Meerpilzes auf die unmittelbar rechte Pflanze oder Ausgabe einer verwendbaren Meerpilzkarte.
- **Sea-shroom + Cactus = Urchin-shroom** ([Seeigelpilz](references/urchin-shroom/port-notes.md)): reihengebundene verfolgende Bodensporen oder ausschließlich gegen Luftziele wirksame Stacheln mit je 20 Schaden. Luftziele haben Vorrang. Nach 120 Sekunden vier Schüsse je Angriff.
- **Sea-shroom + Fume-shroom = Shark-shroom** ([Haipilz](references/shark-shroom/port-notes.md)): 20 schilddurchdringender Rauchschaden alle 1,5 Sekunden. Meerpilzfutter erhöht die Reichweite zehn Sekunden auf die ganze Reihe. Karte, Handschuh und Minnow unmittelbar links können füttern. Die quellseitige Kraken-Rückstoßsynergie bleibt mangels Kraken-shroom unerreichbar.
- **Sea-shroom + Starfruit = Sea Star** ([Meersternpilz](references/sea-star/port-notes.md)): fünf Wassersterne mit je 20 Schaden alle 1,5 Sekunden; nach 60 Sekunden zwei vollständige Salven mit insgesamt zehn Sternen.

Vier Originalmodelle, sämtliche 17 Originalclips, drei Originalgeschosse und vollständige deutsche/englische Almanachtexte mit Originalporträts sind enthalten. Alle vier sind Wasserpflanzen und verwenden native Schlaf-/Kaffeeregeln. Die drei niedrigen Varianten lassen gegnerische geradlinige Erbsen vorbeifliegen; der hohe Haipilz und Basketballtreffer bleiben ausgenommen. Native Speicherfelder erhalten Wachstum, Vermehrung, Reichweitenverstärkung, Angriffsmodus und Flugzustände. Dünger heilt und begrenzt positive Fähigkeitstimer auf höchstens 0,5 Sekunden, einschließlich späterer Vermehrung und laufender Reichweitenverstärkung. Kürzere Timer und Schussphasen bleiben erhalten.

Der Almanach enthält **210 Ergebnisse**. Aktiv sind **260 Rezepte**. Vier erledigte Zeilen entfallen aus `fusionen.txt`; **409 Rezepte mit 341 Ergebnissen** bleiben offen. Die mittlere Versionsstelle steigt einmal auf **2.18.0**. Workflow und Quellenarchiv bleiben unverändert; eine Zeitersparnis wurde nicht nachgewiesen.

[Gemeinsamer Quellen-, Zustands- und Prüfbericht](references/sea-fusions-2-18-0/report.md), [Adapter und Benutzerkommandos](references/sea-fusions-2-18-0/adapters.json), [Installationsbeleg](references/release-2.18.0.json). Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft Wasserterrain, beide Rezeptreihenfolgen, Handschuh, Futterkosten/Kartenverbrauch, Wachstum, alle Mehrfachschüsse, Ballonziele, Vermehrung, Dünger, Pause/Tod, echtes Speichern/Laden und beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.17.0.dll` nach `Mods/FusionMod.dll` kopieren. Danach einen Spielstand ohne neue Kennungen laden. Der vorherige Projektstand liegt unter `backup/sea-fusions-2.17.0.tar.gz`.

## Änderung 2.17.0: Vier Katzenschwanz- und Hypnosefusionen

Vier neue Ergebnisse verwenden ausschließlich bereits native Zutaten. Jedes besitzt genau ein Quellenrezept, registriert für beide Kartenreihenfolgen und Handschuhrichtungen. Keine Variante existiert bereits nativ.

- **Cattail + Ice-shroom = Frost Cattail** ([Frostkatzenschwanz](references/frost-cattail/port-notes.md)): zwei Lenkgeschosse mit je 40 Schaden alle 1,5 Sekunden; doppelter Schaden gegen unterkühlte, vierfacher gegen eingefrorene Ziele.
- **Cattail + Jalapeno = Flame Cattail** ([Flammenkatzenschwanz](references/flame-cattail/port-notes.md)): zwei Lenkgeschosse mit je 40 Schaden alle 1,5 Sekunden; dreifacher Grundschaden gegen bereits überhitzte Ziele vor deren vorhandenem Schadensbonus.
- **Melon-pult + Hypno-shroom = Hypno Melon** ([Hypnomelone](references/hypno-melon/port-notes.md)): 120 Direktschaden alle drei Sekunden und Melonen-Flächenschaden. Jedes überlebende Ziel unter halber Körper- und Helmgesundheit hat nach dem Schaden 25 % Hypnosechance. Bisse hypnotisieren und verwandeln die Pflanze in eine gewöhnliche Melonenpulte zurück.
- **Hypno-shroom + Jalapeno = Hypno Pepper** ([Hypnochili](references/hypno-pepper/port-notes.md)): explodiert nach etwa 0,84 Sekunden oder beim Zerquetschen. Hypnotisiert Ziele derselben Reihe unter 900 Körper- und Helmgesundheit; andere Ziele und gescheiterte Hypnose erhalten 1800 Ascheschaden.

Vier Originalmodelle, sämtliche Originalclips, drei Originalgeschosse und vollständige englische/deutsche Almanachtexte mit Originalporträts sind enthalten. Native Speicherfelder erhalten Angriffsphasen, Zündfortschritt, Geschossfamilien und Schaden. Dünger heilt ohne Sonderentwicklung oder Zurücksetzen laufender Phasen. Katzenschwanzgeschosse nutzen vorhandene Statuszustände; sie erzeugen selbst keine Kälte oder Überhitzung. Abenteueraufwertungen bleiben ausgeschlossen.

Der Almanach enthält **206 Ergebnisse**. Aktiv sind **256 Rezepte**. Vier erledigte Zeilen entfallen aus `fusionen.txt`; **413 Rezepte mit 345 Ergebnissen** bleiben offen. Die mittlere Versionsstelle steigt einmal auf **2.17.0**. Workflow und Quellenarchiv bleiben unverändert; eine Zeitersparnis wurde nicht nachgewiesen.

[Gemeinsamer Quellen-, Zustands- und Prüfbericht](references/mixed-fusions-2-17-0/report.md), [Adapter und Benutzerkommandos](references/mixed-fusions-2-17-0/adapters.json), [Installationsbeleg](references/release-2.17.0.json). Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft Fähigkeiten, Zielsuche in allen Reihen, Flugbahnen, Hypnoseschwellen, Biss-Rückverwandlung, beide Rezeptreihenfolgen, Handschuh, Dünger, echtes Speichern/Laden, Pause/Tod und beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.16.0.dll` nach `Mods/FusionMod.dll` kopieren. Danach einen Spielstand ohne neue Kennungen laden. Der vorherige Projektstand liegt unter `backup/mixed-fusions-2.16.0.tar.gz`.


## Änderung 2.16.0: Vier Sternfusionen

Vier neue Ergebnisse verwenden ausschließlich bereits native Zutaten. Jedes besitzt genau ein Quellenrezept; beide Kartenreihenfolgen und Handschuhrichtungen verwenden dieselbe Rezepttabelle. Keine Variante existiert bereits in Replanted.

- **Starfruit + Garlic = Starlic** ([Knoblauchstern](references/starlic/port-notes.md)): fünf Sterne mit je 30 Schaden alle 1,5 Sekunden. Fünf Zusatzschaden je vorhandenem Giftpunkt, danach ein neuer Giftpunkt. Bisse lösen verzögert einen Reihenwechsel aus.
- **Starfruit + Ice-shroom = Winter Starfruit** ([Winterstern](references/winter-starfruit/port-notes.md)): fünf spiralförmige Eissterne alle 1,5 Sekunden. Je 20 Schaden, zehn Frostpunkte und vierfacher Schaden gegen eingefrorene Ziele.
- **Starfruit + Jalapeno = Spicy Starfruit** ([Chilistern](references/spicy-starfruit/port-notes.md)): explodiert nach einer Sekunde oder beim Tod. Erzeugt eine Feuerreihe und 25 Sterne mit je 100 Schaden. Keine sofortige Feuerreihe bei der Fusion.
- **Peashooter + Starfruit = Starshooter** ([Sternerbsenkanone](references/starshooter/port-notes.md)): eine 20-Schaden-Erbse alle 1,5 Sekunden. Solange sie fliegt, setzt sie nach 0,2/0,7/1,2/1,7/2,2 Sekunden je fünf 20-Schaden-Sterne frei.

Vier Originalmodelle, sämtliche Originalclips, vier Originalgeschosse und vollständige deutsche/englische Almanachtexte mit Originalporträts sind enthalten. Native Speicherfelder erhalten Angriffsphasen, Zündschnur, Geschossfamilien, Schaden, Spiralursprung und Emissionszähler. Bestehende Statusbesitzer speichern Gift und Frost. Dünger heilt ohne Sonderentwicklung. Beim Chilistern begrenzt er die verbleibende Zündzeit auf höchstens 0,5 Sekunden; kürzere Zündzeiten und Schussphasen bleiben erhalten.

Der Almanach enthält **202 Ergebnisse**. Aktiv sind **252 Rezepte**. Vier erledigte Rezeptzeilen entfallen aus `fusionen.txt`; **417 Rezepte mit 349 Ergebnissen** bleiben offen. Die mittlere Versionsstelle steigt einmal auf **2.16.0**. Workflow und Quellenarchiv bleiben unverändert; eine Zeitersparnis wurde nicht nachgewiesen.

[Gemeinsamer Quellen-, Zustands- und Prüfbericht](references/star-fusions-2-16-0/report.md), [Adapter und Benutzerkommandos](references/star-fusions-2-16-0/adapters.json), [Installationsbeleg](references/release-2.16.0.json). Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft Fähigkeiten, Flugbahnen, beide Rezeptreihenfolgen, Handschuh, Dünger während laufender Phasen, echtes Speichern/Laden, Pause/Tod und beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.15.0.dll` nach `Mods/FusionMod.dll` kopieren. Danach einen Spielstand ohne neue Kennungen laden. Der vorherige Projektstand liegt unter `backup/star-fusions-2.15.0.tar.gz`.


## Änderung 2.15.0: Vier Knoblauch- und Melonenfusionen

Vier neue Ergebnisse verwenden ausschließlich bereits verfügbare Zutaten. Alle vier Quellenrezepte sind in beiden Kartenreihenfolgen und Handschuhrichtungen registriert. Keine dieser Pflanzen existiert nativ in Replanted.

- **Cabbage-pult + Garlic = Garbage-pult** ([Knoblauchkohlapult](references/garbage-pult/port-notes.md)): 40 Schaden alle drei Sekunden und 20 Zusatzschaden je vorhandenem Giftpunkt. Vergibt danach einen Giftpunkt. Ein zusätzlicher Abpraller; Ziele mit vorhandenen Giftpunkten verbrauchen keinen Abpraller.
- **Kernel-pult + Garlic = Clove-pult** ([Knoblauchkornapult](references/clove-pult/port-notes.md)): 40 Schaden, gleicher Giftbonus. Mit 25 % Wahrscheinlichkeit Knoblauchbutter mit 60 Grundschaden, vier Sekunden Butter und zehn Sekunden Vergiftung.
- **Melon-pult + Garlic = Garlic-pult** ([Knoblauchmelonapult](references/garlic-pult/port-notes.md)): 100 Direktschaden mit 20 Zusatzschaden je Giftpunkt. Flächenschaden erhält fünf Zusatzschaden je Giftpunkt. Überlebende erhalten danach jeweils einen Giftpunkt.
- **Melon-pult + Jalapeno = Summer Melon** ([Sommermelone](references/summer-melon/port-notes.md)): 120 Direktschaden und Melonen-Flächenschaden. Überhitzt überlebende direkte Ziele und Flächenziele nach dem Schaden.

Originalgrafik, alle Aktionsclips, fünf Originalgeschosse, Originalporträts und vollständige deutsche/englische Almanachtexte sind enthalten. Dünger heilt ohne Sonderentwicklung, neue Butterwahl oder Zurücksetzen laufender Angriffe. Native Pflanzen- und Geschossfelder speichern Phasen, Butterwahl, Schaden und Abpraller. Die vorhandenen gespeicherten Statusdatensätze erhalten Giftpunkte/Giftzeitgeber und die direkte beziehungsweise durch Flächentreffer verursachte Überhitzung. Weitergegebene Überhitzung aus der bestehenden Todesexplosion behält die bisherige Speichergrenze.

Der Almanach enthält **198 Ergebnisse**. Aktiv sind **248 Rezepte**. Vier erledigte Rezeptzeilen entfallen aus `fusionen.txt`; **421 Rezepte mit 353 Ergebnissen** bleiben offen. Die mittlere Versionsstelle steigt einmal auf **2.15.0**. Workflow und Quellenarchiv bleiben unverändert; eine Zeitersparnis wurde nicht nachgewiesen.

[Gemeinsamer Quellen-, Zustands- und Prüfbericht](references/garlic-lobbers-2-15-0/report.md), [Adapter und Benutzerkommandos](references/garlic-lobbers-2-15-0/adapters.json), [Installationsbeleg](references/release-2.15.0.json). Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft Fähigkeiten, Projektile, beide Rezeptreihenfolgen, Handschuh, Dünger, echtes Speichern/Laden, Pause/Tod sowie beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.14.0.dll` nach `Mods/FusionMod.dll` kopieren. Danach einen Spielstand ohne die neuen Kennungen laden. Der vorherige Projektstand liegt unter `backup/garlic-lobbers-2.14.0.tar.gz`.


## Änderung 2.14.0: Vier Rauchpilzfusionen

Vier neue Ergebnisse verwenden bereits verfügbare Zutaten. Alle vier Quellenrezepte sind in beiden Kartenreihenfolgen und Handschuhrichtungen registriert. Keine dieser Varianten existiert nativ in Replanted.

- **Fume-shroom + Peashooter = Boost-shroom** ([Schubpilz](references/boost-shroom/port-notes.md)): 20 durchdringender Schaden alle 1,5 Sekunden über die gesamte vordere Reihe. Während des ursprünglichen Rauchfensters steigen Schaden und Geschwindigkeit geradliniger Geschosse vor ihm alle 0,02 Sekunden. Die Boni sind stapelbar und werden gespeichert.
- **Fume-shroom + Torchwood = Blowtorch-shroom** ([Brennerpilz](references/blowtorch-shroom/port-notes.md)): Absorbiert gewöhnliche Erbsen bis 300 gespeicherter Energie und entlädt sie höchstens einmal pro Sekunde als durchdringenden Flammentreffer. Schneeerbsen werden aufgetaut und fliegen durch. Die tatsächliche Quellenreichweite beträgt sieben Quelleinheiten.
- **Fume-shroom + Wall-nut = Wallop-shroom** ([Schlagpilz](references/wallop-shroom/port-notes.md)): 4000 HP. Verbraucht einen verfügbaren Nusspilz im Bereich von 3 × 3 Feldern. Sein Originalgeschoss erhält 20 plus dessen aktuelle Gesundheit als Schaden, umgeht Schilde und durchdringt schwächere Gegner mit entsprechend sinkendem Restschaden.
- **Fume-shroom + Spikeweed = Caltrop-shroom** ([Nagelpilz](references/caltrop-shroom/port-notes.md)): Niedrige, nicht blockierende Pflanze. 40 durchdringender Rauchschaden alle 1,5 Sekunden; gewöhnliche Fahrzeuge erhalten zusätzlich 30 % ihrer maximalen Körper- und Rüstungsgesundheit. Fahrzeugkontakt zersticht Reifen und verbraucht die Pflanze.

Originalmodelle, alle Aktionsclips, das neue Nusspilzgeschoss, Originalporträts und vollständige deutsche/englische Almanachtexte sind enthalten. Native Rauchpartikel ersetzen die ursprünglichen Rauch-/Flammenpartikel. Dünger heilt ohne Sonderentwicklung oder Zurücksetzen laufender Angriffe, gespeicherter Munition und Energie. Die mittlere Versionsstelle steigt einmal auf **2.14.0**.

Der Almanach enthält **194 Ergebnisse**. Vier erledigte Rezeptzeilen entfallen aus `fusionen.txt`; **425 Rezepte mit 357 Ergebnissen** bleiben offen. Workflow und Quellenarchiv bleiben unverändert. Eine Zeitersparnis wurde nicht nachgewiesen.

[Gemeinsamer Quellen-, Zustands- und Prüfbericht](references/fume-expansion-2-14-0/report.md), [betroffene Adapter und Benutzerkommandos](references/fume-expansion-2-14-0/adapters.json), [Installationsbeleg](references/release-2.14.0.json). Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft beide Rezeptreihenfolgen, Handschuh, Terrain/Unterlagen/Kürbis, alle Fähigkeiten und Projektile, echtes Speichern/Laden, Pause/Tod, Dünger und beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.13.0.dll` nach `Mods/FusionMod.dll` kopieren. Danach einen Spielstand ohne neue Kennungen laden. Der vorherige Projektstand liegt unter `backup/fume-expansion-2.13.0.tar.gz`.



## Änderung 2.13.0: Vier Wehblattfusionen

Vier neue Ergebnisse verwenden ausschließlich bereits native Zutaten. Jedes Ergebnis besitzt genau ein Quellenrezept; beide Kartenreihenfolgen und Handschuhrichtungen sind aktiv. Keine dieser Varianten existiert bereits nativ in Replanted.

- **Blover + Blover = Cyclover** ([Wirbelblatt](references/cyclover/port-notes.md)): Schiebt beim Originalclip bis zu drei nächste Gegner in derselben Reihe zurück. Der Schub berücksichtigt Flug, Hochwerfen, Fahrzeuge und beide nativen Giganten. Nur der erste Angriff vertreibt Nebel und Ballonzombies.
- **Blover + Garlic = Disguster** ([Knoblauchweher](references/disguster/port-notes.md)): Vergibt mindestens fünf Giftpunkte, danach einen weiteren Punkt und fünf Sekunden Vergiftung. Erzwingt zulässige Reihenwechsel und reagiert auch auf Bisse. Verschwindet zwei Sekunden nach dem Blasen.
- **Blover + Ice-shroom = Breeze Blover** ([Frostweher](references/breeze-blover/port-notes.md)): Zehn Kälteimpulse mit je einem Schaden und 20 Frostpunkten. Wandelt vorhandene und während des Winds neu erzeugte Schneeerbsen in vollständige Polargeschosse um: erhaltene Flugbahn und Schadensboni, 20 Frostpunkte, vierfacher Schaden gegen eingefrorene oder kälteimmune Ziele, keine Umwandlung durch gewöhnliches Fackelholz.
- **Blover + Hypno-shroom = Mind-blover** ([Hypnoweher](references/mind-blover/port-notes.md)): Zieht hypnotisierte Zombies fünf Sekunden zurück. Beim Start kostet dies einmalig 10 % ihrer maximalen Körper- und Helmgesundheit. Die linke Randspalte bleibt ausgenommen. Flug, Fahrzeuge und Giganten besitzen die belegten Geschwindigkeitsfaktoren.

Originalmodelle, sämtliche Originalclips, Polargeschosse, Originalporträts und vollständige englische/deutsche Almanachtexte sind enthalten. Native Pflanzen- und Geschossfelder speichern Phasen, Lebensdauer und Geschossschaden. Unsichtbare gespeicherte GridItems erhalten die von diesen Fusionen erzeugten Gift- und Frostzustände einschließlich steigendem Frostschwellenwert. Die bestehenden Statusbesitzer bleiben für die Wirkung zuständig. Dünger heilt ohne Entwicklung, zusätzliche Angriffe oder Zurücksetzen laufender Zustände.

Der Almanach enthält **190 Ergebnisse**. Vier erledigte Rezeptzeilen entfallen aus `fusionen.txt`; **429 Rezepte mit 361 Ergebnissen** bleiben offen. Workflow und Quellenarchiv bleiben unverändert. Eine Zeitersparnis wurde nicht nachgewiesen. Die mittlere Versionsstelle steigt einmal auf **2.13.0**.

[Gemeinsamer Quellen-, Zustands- und Prüfbericht](references/blover-fusions-2-13-0/report.md), [betroffene Adapter und Benutzerkommandos](references/blover-fusions-2-13-0/adapters.json) und [Installationsbeleg](references/release-2.13.0.json). Statische Prüfungen und Kompilierung sind von Laufzeitprüfungen zu unterscheiden. Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft beide Rezeptreihenfolgen, Handschuh, Unterlagen/Kürbis, Kosten/Cooldown, alle Fähigkeiten und Polargeschosse, Pause/Tod, echtes Speichern/Laden, Dünger sowie beide Almanachsprachen. Windpartikel verwenden native FumeCloud-Effekte; Ursprungsgrafik und Aktionsclips bleiben erhalten.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.12.0.dll` nach `Mods/FusionMod.dll` kopieren. Anschließend einen Spielstand ohne die neuen Kennungen laden. Code, Werkzeuge, Tests, Projektdatei, README, Rezeptliste und Prüfsummen vor der Änderung liegen unter `backup/blover-fusions-2.12.0.tar.gz`.

## Änderung 2.12.0: Augen-Zombie und Blutsonnenblume

Der Augen-Zombie aus PvZ-Scared ersetzt zufällig 1 % frisch erzeugter ungeschützter Browncoats. Er besitzt 270 HP, wacht bei nahen feindlichen Pflanzen, Zombies oder Projektilen auf und verursacht danach 50 Schaden je Biss nach dem Originalanimationstakt. Bei gewöhnlichem Tod besteht eine unabhängige Kopfchance von 1 %; Instakills erzeugen keinen Kopf. Die erwartete Gesamtquote beträgt damit 0,01 % der ursprünglichen Browncoats bei ausschließlich zulässigen Todesarten.

**Augen-Zombie-Kopf + normale Sunflower = Blutsonnenblume.** Der Kopf ist eine lose Handschuhzutat und lässt sich nicht auf ein freies Feld pflanzen. Die Blutsonnenblume besitzt 300 HP und erzeugt alle 25 Sekunden 25 Sonne. Wie ausdrücklich gewünscht verwandelt sie nach zehn ununterbrochenen Sekunden die Pflanze direkt rechts von ihr in eine weitere Blutsonnenblume. Unterlagen und Kürbisse bleiben erhalten; Zielwechsel setzt den Fortschritt zurück. Die erste Pflanze benötigt die Kopffusion.

Originalgrafik, Originalanimationen, Originalporträt und englische/deutsche Almanachtexte sind enthalten. Originalton wird auf Benutzerwunsch nicht verwendet; das Erwachen nutzt einen nativen Replanted-Wutschrei. Zombiephasen, Dropentscheidung, lose Köpfe und Infektionsfortschritt verwenden native Speicherfelder. Der Almanach umfasst 186 Ergebnisse. Aktiv sind 236 Rezepte; offen bleiben 433 Rezepte mit 365 Ergebnissen. Die Einführung neuer Einheiten erhöht die mittlere Versionsstelle einmal auf **2.12.0**.

[Quellen-, Zustands- und Prüfbericht](references/scared-2-12-0/report.md), [Adapterkommandos](references/scared-2-12-0/adapters.json), [letzte Adapter-Builds](references/scared-2-12-0/adapter-build-final.log) und [Installationsbeleg](references/release-2.12.0.json). Spiel und Testprogramme bleiben geschlossen; der Benutzer übernimmt sämtliche Laufzeitprüfungen. Besonders wichtig sind Todesarten, echter Spielstand mit losem Kopf, Biss-/Schutzinteraktionen, beide Handschuhrichtungen und Infektion. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.11.1.dll` nach `Mods/FusionMod.dll` kopieren und einen Spielstand ohne neue Kennungen laden.

## Änderung 2.11.1: Fusionspartner per Rechtsklick

Ein Rechtsklick auf einen freigeschalteten normalen Pflanzeneintrag öffnet den zusätzlichen Almanach-Tab „Mit: Pflanzenname“. Er zeigt alle unterschiedlichen Zutatenpflanzen, die mit dieser Pflanze über ein aktuell implementiertes Rezept fusionieren können. Auch Fusionspflanzen, Selbstfusionen, Eimer und Football-Helm werden berücksichtigt. Die Detailansicht nennt jeweils die ursprüngliche Pflanze, die gewählte Zutat und das Ergebnis. Gemeint sind Zutatenpartner; der bestehende Tab „Fusions“ enthält weiterhin alle 185 Ergebnisse.

Die Liste stammt unmittelbar aus `FusionRecipes`, derselben ungeordneten Rezepttabelle wie Karten und Handschuh. Native Karten werden über ihren gebundenen `AlmanacEntryModel` identifiziert, unabhängig von Rasterposition oder übersetztem Namen. Native Namen und Vorschaubilder sowie vorhandene Fusionsporträts werden wiederverwendet. Eimer und Football-Helm verwenden ihre bereits eingebetteten Original-Körpersprites. Nicht implementierte Rezepte werden nicht angezeigt. Ohne passende Rezepte erscheint eine ausdrückliche Leermeldung. Ein weiterer Rechtsklick in der normalen Pflanzenansicht ersetzt den Kontext des zusätzlichen Tabs.

„Pflanzen“ stellt die normale Ansicht wieder her; der zusätzliche Tab öffnet dieselbe Partnerliste erneut. Der bestehende Fusionskatalog, Seitenwechsel, englische/deutsche Beschriftungen, Bildfreigabe, native Schließenaktionen und Controller-Nachbarziele bleiben in der gemeinsamen Ansicht. Die Änderung betrifft ausschließlich die Almanachbedienung; weder neue Pflanzen noch Gameplayänderungen werden eingeführt. Deshalb steigt die letzte Versionsstelle auf **2.11.1**.

Statische Konsistenzprüfung und Release-Build; kein Spielstart und keine Testprogramme. Installation und Sicherung: [Releasebeleg](references/release-2.11.1.json). Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.11.0.dll` nach `Mods/FusionMod.dll` kopieren. Vorherige geänderte Projektdateien: `backup/almanac-partners-2.11.0/`.

Manuelle Prüfung durch den Benutzer: Rechtsklick auf Peashooter (37 unterschiedliche Partner einschließlich Peashooter und Eimer), Puff-shroom (20 Partner), Tall-nut (Football-Helm) sowie eine Pflanze ohne aktives Rezept. Kartenwahl muss passende Ergebnisse nennen. Linksklick, Wechsel zu „Pflanzen“ und „Fusions“, erneuter Rechtsklick auf eine andere Pflanze, erneutes Öffnen/Schließen, lange Namen, Sprachwechsel und Controller-Navigation gegenprüfen. Tatsächliche Darstellung und Eingabeverhalten sind durch Kompilierung nicht bestätigt.

## Änderung 2.11.0: Vier Wassergrasfusionen

Vier neue Ergebnisse wurden nacheinander mit ihren benötigten Fähigkeiten umgesetzt. Alle Zutaten waren bereits nativ verfügbar; jede Pflanze besitzt genau ein Quellenrezept. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden den vorhandenen Fusionsweg. Keine der vier Varianten existiert nativ in Replanted.

- **Tangle Kelp + Puff-shroom = Tingle Kelp** ([Kribbelalge](references/tingle-kelp/port-notes.md)): Bei Annäherung platzt sie nach dem Originalclip und erzeugt 30 ballistische Originalgeschosse mit jeweils 20 Schaden. Zermalmen löst die Salve unmittelbar aus. Wasserzombies erhalten Wassergras; tödliche Wassergrasanwendungen können auf zulässigen freien Wasserfeldern neue Kribbelalgen erzeugen.
- **Tangle Kelp + Fume-shroom = Grapple Kelp** ([Greifalge](references/grapple-kelp/port-notes.md)): Drei durchdringende Treffer mit je 30 Schaden, zusätzlichem Wassergras, punktabhängigem Zusatzschaden und Rückstoß. Der tatsächliche Quellenradius beträgt sieben Quelleinheiten. Die ursprüngliche Controllerfolge, Überblendungen, 1,5-fache Geschwindigkeit des Trefferclips, alle Knochenmeshes und SpriteMasks sind eingebunden.
- **Tangle Kelp + Wall-nut = Tangle-nut** ([Algennuss](references/tangle-nut/port-notes.md)): 4000 HP und echte Blockierung im Wasser. Bisse vergeben Wassergras. Zermalmen tötet sie nicht sofort; ein zermalmender Wasserzombie kostet sie alle 0,02 Sekunden 50 HP und erhält Wassergras. Beschädigte Körper und verlorene Arme folgen der aktuellen Gesundheit.
- **Tangle Kelp + Spikeweed = Spike-kelp** ([Stachelalge](references/spike-kelp/port-notes.md)): 20 durchdringender Kontaktschaden pro Sekunde samt Wassergras. Fahrzeugtreffer kosten sie 10 HP. Die ursprünglichen Zielzustände, Kontaktphase und Wasseranimation bleiben berücksichtigt; Fahrzeugkontakt ist keine sofortige Vernichtung.

Wassergras halbiert das Bewegungstempo, addiert Wirkzeiten und behält Punkte nach Ablauf der Verlangsamung. Ab der siebten Anwendung folgen 1 % der maximalen Körper- und Helmgesundheit als Zusatzschaden. Ab der 31. folgt außerdem ein eigener Treffer mit der gesamten Maximalgesundheit. Bosse sind gegen Wassergras immun. Pflanzen-, Geschoss- und unsichtbare native GridItem-Felder speichern Phasen, Flug, Punkte und Restdauer. Dünger heilt alle Schichten ohne Sonderentwicklung oder Zurücksetzen dieser Zustände. Die Greed-Kelp-Umwandlung benötigt noch nicht implementierte Pflanzen; beide Almanachsprachen benennen diese Grenze.

Originalmodelle, sämtliche Clips, Geschosssprites, Originalporträts und vollständige englische/deutsche Almanachtexte sind enthalten. Der Almanach enthält **185 Ergebnisse**. Vier erledigte Rezeptzeilen entfallen aus `fusionen.txt`; **433 Rezepte mit 365 Ergebnissen** bleiben offen. Workflow und Quellenarchiv bleiben unverändert. Eine Zeitersparnis wurde nicht nachgewiesen.

[Quellen-, Speicher- und Releasebericht](references/kelp-fusions-2-11-0/report.md), [betroffene Adapter samt Benutzerkommandos](references/kelp-fusions-2-11-0/adapters.json) und [Installationsbeleg](references/release-2.11.0.json). Spiel und Testprogramme wurden nicht gestartet. Der Prüfumfang umfasst statische Konsistenz, Originalexport, visuelle Vorschauen und Kompilierung. Der Benutzer prüft insbesondere Wasserterrain, beide Rezept-/Handschuhrichtungen, Unterlagen/Kürbis, sämtliche Angriffe, Schwellen und Vermehrung, Knochen/Masken, echte Speicherung, Pause/Tod, Dünger und beide Almanachsprachen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.10.0.dll`, danach einen Spielstand ohne neue Pflanzen-, Geschoss- oder Statuseinträge laden. Der vorherige Projektstand liegt in `backup/kelp-fusions-2.10.0.tar.gz`. Die mittlere Versionsstelle steigt für vier neue Pflanzen einmal auf **2.11.0**.

## Änderung 2.10.0: Vier aktive Nussfusionen

Vier neue Ergebnisse verwenden ausschließlich bereits native Zutaten. Jedes hat genau ein Quellenrezept; beide Kartenreihenfolgen und Handschuhrichtungen sind aktiv:

- **Wall-nut + Cabbage-pult = Cabbage-nut** ([Nusssalat](references/cabbage-nut/port-notes.md)): 4000 HP, speichert tatsächlichen Gesundheitsschaden mit den Quellenmultiplikatoren und wirft bei mehr als 1000 Energie einen gleich starken Kohl. Ein gleich starker, abklingender Schild fängt numerische Verletzungen ab.
- **Wall-nut + Melon-pult = Melon-nut** ([Melonuss](references/melon-nut/port-notes.md)): 8000 HP, 1,5-fache Energie, Grenze 32000. Wirft die Energie als Melone einschließlich vollständigem Flächenschaden und erhält den gleichen Schild.
- **Wall-nut + Threepeater = Three-nuts** ([Triple-Nuss](references/three-nuts/port-notes.md)): 12000 HP, hohe Blockierung und Rollnüsse mit 80/60/40 Schaden für insgesamt 3/5/7 verschiedene Ziele. Verlorene Köpfe bleiben verloren; auch Dünger heilt danach höchstens auf 8000 beziehungsweise 4000 HP.
- **Wall-nut + Squash = Squash-nut** ([Matschnuss](references/squash-nut/port-notes.md)): 4000 HP, Sprünge auf dem eigenen Feld und 160 Quetschschaden bei Landung. In der Luft keine Blockierung oder Bisse. Der Originalclip dauert 1,666667 s; der übersetzte Almanachwert von 1,5 s wird korrigiert.

Alle Fähigkeiten, Originalmodelle, sämtliche Clips, beschädigte Körper, vorhandene Originalprojektile und vollständige englische/deutsche Almanachtexte mit Originalporträts sind eingebunden. Keine neue Variante existiert nativ in Replanted. Gespeicherte Pflanzen- und Geschossfelder erhalten Energie, Schild, Phasen und Kopfverlust. Unsichtbare native GridItems speichern die vollständigen Zielhandles der Rollgeschosse und verhindern wiederholte Treffer auf überlappende Zombies auch nach Laden.

Dünger heilt alle Schichten ohne Sonderentwicklung, Energieaufladung, Schildauffüllung, Phasenrücksetzung oder Zusatzschüsse. Triple-Nuss beachtet die dauerhafte Heilgrenze. Der Almanach enthält **181 Ergebnisse**. Vier erledigte Zeilen entfallen aus `fusionen.txt`; offen bleiben **437 Rezepte mit 369 Ergebnissen**. Workflow und Quellenarchiv bleiben unverändert.

[Gemeinsamer Quellen-, Gesundheits- und Zustandsvertrag](references/nut-fusions-2-10-0/report.md), [betroffene Adapter und Benutzerkommandos](references/nut-fusions-2-10-0/adapters.json) und [Buildprotokoll](references/nut-fusions-2-10-0/adapter-build.log) dokumentieren die Umsetzung. Prüfumfang: statische Konsistenz, Originalexport, visuelle Vorschauen und Kompilierung. **Spiel und Testprogramme werden nicht gestartet.** Der Benutzer prüft Rezepte und Handschuh, Kosten/Cooldown, Dach/Unterlagen/Kürbis, Schilde, Schaden und Flächenschaden, Kopfverluste samt Dünger, Durchdringungen, Sprungblockierung, echte Speicherung, Pause/Tod und beide Almanachsprachen.

Installationsbeleg nach Abschluss: `references/release-2.10.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.9.0.dll`; vorheriger Projektstand: `backup/nut-fusions-2.9.0.tar.gz`. Danach einen Spielstand ohne neue Pflanzen, Geschosskennungen oder Trefferhistorien laden. Die mittlere Versionsstelle steigt für vier neue Pflanzen auf **2.10.0**.

## Änderung 2.9.0: Frostkohl, Kimchi, Eismais und Salatwerfer

Vier neue Ergebnisse mit allen sechs verfügbaren Quellenrezepten sind implementiert:

- **Cabbage-pult + Ice-shroom = Coldslaw** ([Frostkohl](references/coldslaw/port-notes.md)): 40 Schaden, zehn Gefrierpunkte vor der Vereisungsprüfung, vierfacher Schaden und weitere Abpraller bei vereisten Treffern. Der lebende Erzeuger erhält drei Sekunden lang dreifache Animationsgeschwindigkeit und Angriffsintervalle von 0,2 Sekunden; danach folgen 1,5 Sekunden Sperrzeit.
- **Cabbage-pult + Jalapeno = Kimchi-pult** ([Kimchi-Werfer](references/kimchi-pult/port-notes.md)): 60 Schaden und Überhitzung vor dem Treffer. Direkte Treffer erzeugen bis zu neun kleine Geschosse mit jeweils 20 Basisschaden im 3×3-Bereich. Splitter behalten Überhitzung und teilen sich nicht erneut.
- **Kernel-pult + Ice-shroom = Cobsicle** ([Eismais](references/cobsicle/port-notes.md)): 20er-Eiskörner oder 40er-Eisbutter, 25 % Butterchance, vierfacher Schaden gegen bereits vereiste Ziele. Mais vergibt Gefrierpunkte nach dem Schaden; Butter friert danach vier Sekunden ein.
- **Cabbage-pult + Cob-pult**, **Kernel-pult + Overripe Melon** oder **Melon-pult + Taco-pult = Salad-pult** ([Salatwerfer](references/salad-pult/port-notes.md)): 140 Schaden samt Flächenschaden, 42 % Chance auf verkettbare Folgeangriffe. Ein Direkttreffer erzeugt eine Überreife Melone und eine Mais- oder Buttermelone mit sämtlichen vorhandenen Abprall-, Flächen- und Butterfähigkeiten.

Alle Zutaten waren bereits implementiert. Beide Kartenreihenfolgen und Handschuhrichtungen sind aktiv. Keine der vier Pflanzen existiert nativ in Replanted. Kohltopf unterstützt die neuen Kohlvarianten und Salatwerfer; der aktive Frostkohltakt hat Vorrang. Buttertopf erhöht Eismais- und Salatbutter auf 50 %. Melonentopf stärkt Salat und beide Nachfolger. Dünger heilt alle Schichten ohne Sonderentwicklung und verändert weder Phasen, Frostkohlbonus, Sperrzeit, Butterwahl noch fliegende Geschosse.

Vier Originalpflanzen und fünf neue Originalgeschossmodelle mit allen Clips, Ebenen, Schatten und Rotationen sind eingebunden. Salatnachfolger verwenden zusätzlich drei bestehende Originalmodelle. Vollständige englische und deutsche Almanachtexte enthalten Originalporträts, Begleittexte und belegte Abweichungen gegenüber ungenauen Quellenbeschreibungen. Der Almanach umfasst **177 Ergebnisse**. Sechs erledigte Zeilen entfallen aus `fusionen.txt`; offen bleiben **441 Rezepte mit 373 Ergebnissen**. Workflow und Quellenarchiv bleiben unverändert.

[Gemeinsamer Quellen- und Zustandsvertrag](references/elemental-lobbers-2-9-0/report.md), [betroffene Adapter und Benutzerkommandos](references/elemental-lobbers-2-9-0/adapters.json) sowie [Buildprotokoll](references/elemental-lobbers-2-9-0/adapter-build.log) dokumentieren die Umsetzung. Neue Angriffs- und Geschosszustände werden nativ gespeichert. Die bestehenden flüchtigen Gefrierpunkte/-schwellen und Überhitzung überleben einen Spielneustart weiterhin nicht; beide Almanachsprachen benennen diese Grenze.

Prüfumfang: statische Konsistenz, Originalexport, visuelle Vorschauen und Kompilierung. **Spiel und Testprogramme werden nicht gestartet.** Der Benutzer prüft insbesondere sämtliche Rezepte und Handschuhrichtungen, Dach/Unterlagen/Kürbis, Kosten/Cooldown, Eis- und Feuerwirkungen, Frostkohlphasen, alle kleinen und großen Geschosse, Salatketten und Nachfolger, Topfboni, Dünger, Zielverlust, Pause/Tod, echtes Speichern/Laden und Almanach auf Deutsch/Englisch.

Installationsbeleg nach Abschluss: `references/release-2.9.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.8.0.dll`; vorheriger Projektstand: `backup/elemental-lobbers-2.8.0.tar.gz`, ergänzende vorherige Almanachdaten: `backup/elemental-lobbers-almanac-2.8.0.tar.gz`. Danach einen Spielstand ohne neue Pflanzen- und Geschosskennungen laden. Die mittlere Versionsstelle steigt für vier neue Pflanzen auf **2.9.0**.

## Änderung 2.8.0: Vier weitere Werferfusionen

Neu sind **Cabbage-pult + Kernel-pult = Taco-pult**, **Kernel-pult + Melon-pult = Cob-pult**, **Cabbage-pult + Melon-pult = Overripe Melon** und **Cabbage-pult + Threepeater = Cabbage Tri-pult**. Alle Zutaten waren bereits nativ verfügbar. Jedes Ergebnis besitzt genau dieses eine Quellenrezept. Beide Kartenreihenfolgen und Handschuhrichtungen sind aktiv. Keine der vier Pflanzen existiert nativ in Replanted; die vorhandene Maiskanone bleibt ein eigenes natives Ergebnis.

- [Taco-pult](references/taco-pult/port-notes.md) wirft alle 3 s Tacos mit 40 oder Buttertacos mit 60 Schaden. Beide treffen bis zu drei verschiedene Zombies. Butter hält vier Sekunden fest. Jeder Abpraller halbiert die Fluggeschwindigkeit; die Landung beendet das Geschoss.
- [Cob-pult](references/cob-pult/port-notes.md) wirft alle 3 s Maismelonen mit 140 oder Buttermelonen mit 160 Schaden und Melonenflächenschaden. Butter hält das Direktziel vier Sekunden fest; die Dauer im Umfeld beträgt max(0,5; ganzzahlig 4/Zielzahl) Sekunden. Der Originalcode verwendet 140/160 statt der veralteten englischen Übersetzungswerte 120/140.
- [Overripe Melon](references/overripe-melon/port-notes.md) wirft alle 3 s Melonen mit 80 Direktschaden, einem Abpraller und Flächenschaden an jedem Treffer sowie bei der abschließenden Landung.
- [Cabbage Tri-pult](references/cabbage-tri-pult/port-notes.md) wirft alle 1,5 s Kohl mit 40 Schaden über drei Reihen, mit Randersatz. Nach Treffern folgt genau ein nächstes Ziel einer Nachbarreihe. Seitliche Geschosse springen einmal, das mittlere zweimal; Nachfolgegeschosse fliegen 0,75 s. Die englische Übersetzung beschreibt die Einzelzielregel ungenau.

Kohltopf unterstützt Taco, Überreife Melone und Dreifachkohl; Buttertopf unterstützt Taco und Maismelone. Melonentopf verstärkt die beiden neuen Melonen mit dem belegten Faktor 0,67. Dreifachtopf erhöht Dreifachkohlschaden einschließlich Abprallern auf 48. Die bisherigen Familienwerte bleiben erhalten. Die neuen Werfer verwenden die ursprüngliche Intervallschwankung und Schussereignisse bei 0,316667/0,333333/0,25 s, auf native Hundertstelsekundenticks abgebildet. Dünger heilt alle Schichten; keine der vier Varianten besitzt eine Sonderentwicklung. Er verändert weder Phasen, Butterwahl noch fliegende Geschosse.

Vier Pflanzenmodelle und sieben Geschossmodelle einschließlich aller Butter- und Schlussvarianten stammen aus Fusion 4.0.5.2. Originalclips, additive Ebene, Rotation, Schatten, Originalporträts und vollständige englische/deutsche Almanachtexte sind eingebunden. Gespeicherte native Felder besitzen Phasen, Schaden, Topfbonus und bereits getroffene Zielhandles. Die vier erledigten Rezeptzeilen entfallen aus `fusionen.txt`. Offen bleiben **447 Rezepte mit 377 Ergebnissen**; der Almanach umfasst **173 Ergebnisse**. Workflow und `fusionen-quellen.json` bleiben unverändert.

[Gemeinsamer Quellen- und Zustandsvertrag](references/lobber-fusions-2-8-0/report.md), [Adapter und Benutzerkommandos](references/lobber-fusions-2-8-0/adapters.json) und [Buildprotokoll](references/lobber-fusions-2-8-0/adapter-build.log) dokumentieren den Nachweis. Prüfumfang: statische Konsistenz, Originalexport, Vorschauen und Kompilierung. **Spiel und Testprogramme werden nicht gestartet.** Manuell bleiben Karten/Handschuh in beiden Richtungen, Kosten/Cooldown, Terrain/Unterlagen/Kürbis, sämtliche Geschossvarianten und Topfboni, Zielverlust, Butterimmunität, Randreihen, echte Speicherung, Pause/Tod, Animationen und Almanach in beiden Sprachen zu prüfen.

Installationsbeleg: `references/release-2.8.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.7.0.dll`; vorheriger Projektstand einschließlich Tests und Dokumentation: `backup/lobber-fusions-2.7.0.tar.gz`. Danach einen Spielstand ohne neue Pflanzen- und Projektilkennungen laden. Die mittlere Versionsstelle steigt für vier neue Pflanzen auf **2.8.0**.

## Änderung 2.7.0: Zwei Schützentöpfe und zwei Dreifachwerfer

Neu sind **Flower Pot + Peashooter = Pea Pot**, **Flower Pot + Threepeater = Tri-pot**, **Kernel-pult + Threepeater = Kernel Tri-pult** und **Melon-pult + Threepeater = Melon Tri-pult**. Sämtliche Zutaten waren bereits verfügbar. Jedes Ergebnis hat genau ein Quellenrezept; beide Kartenreihenfolgen und Handschuhrichtungen werden unterstützt. Keine dieser vier Varianten existiert nativ in Replanted.

- [Pea Pot](references/pea-pot/port-notes.md) erzeugt nach 30 primären Angriffsereignissen fünf Zusatzangriffe seiner zulässigen Erbsenpflanzen. Diese zählen selbst mit; nach der ersten Auslösung genügen 25 weitere normale Ereignisse. Die ursprünglichen Projektile behalten sämtliche Fähigkeiten und erhalten den originalen zufälligen Mündungsversatz.
- [Tri-pot](references/tri-pot/port-notes.md) erstattet einmalig 100 Sonne, erhöht die Angriffskraft der vorhandenen Dreifachschützenlinie um 20 % und erzeugt nach sechs primären Erbsenangriffen zwei seitliche Angriffe mit halbem Angriffsschaden. Randreihen werden in der eigenen Reihe ersetzt. Fackelholz, Eis, Gift, Hypnose, Eisen und Kirschaufladung bleiben berücksichtigt; feste Explosionen behalten ihren festen Schaden.
- [Kernel Tri-pult](references/kernel-tri-pult/port-notes.md) wirft alle 1,5 s Mais oder Butter über drei Reihen. Nichtbutterserien erhöhen seine Zusatzgeschosse; Butter setzt die Serie anschließend zurück. Die genaue ausgelieferte Zählregel steht in den Portnotizen und beiden Almanachsprachen. Buttertopf und Dreifachtopf wirken auf die neue Variante.
- [Melon Tri-pult](references/melon-tri-pult/port-notes.md) wirft alle 1,5 s drei Melonen mit 80 Direktschaden und vollständigem Flächenschaden. Randersatz, Dreifachtopf und Melonentopf sind berücksichtigt. Beide Werfer übernehmen die ursprüngliche Intervallschwankung, das Schussereignis bei 0,25 s und die ursprüngliche Wurfbahn.

Dünger heilt die neuen Pflanzen einschließlich aller Schichten. Die Quelle enthält für sie keine Sonderentwicklung; Dünger verändert keine Topfzähler, Nichtbutterserie oder Butterauswahl und vergibt weder Sonne noch Zusatzsalven. Gespeicherte Pflanzen- und Projektilfelder erhalten Zähler, gewählte Salven und Angriffsschaden über Laden hinweg. Die beiden Töpfe bewahren den vorhandenen Erzeugungsschutz von 0,5 s. Kartenkosten, Cooldown, Unterlagen und Kürbisse verwenden die vorhandenen Besitzer.

Originalmodelle, sämtliche erforderlichen Clips, drei neue Originalgeschossmodelle und Originalporträts stammen aus Fusion 4.0.5.2. Alle vier Ergebnisse besitzen vollständige englische und deutsche Almanachtexte samt Begleittext. Offen bleiben **451 Rezepte mit 381 Ergebnissen**; der Almanach umfasst **169 Ergebnisse**. `fusionen-quellen.json` und Workflow bleiben unverändert. Ein belegter Bedarf für eine Workflowänderung entstand nicht.

Prüfgrenze: Statische Konsistenz und Kompilierung, keine ausgeführten Testprogramme und kein Spielstart. Die **27 betroffenen Offline-Suites** und ihre Benutzerkommandos stehen in [adapters.json](references/support-pots-2-7-0/adapters.json). Der Release-Build prüft zusätzlich die tatsächlichen nativen Typen und Renderer. Manuell bleiben beide Kartenreihenfolgen und Handschuhrichtungen, Kosten/Cooldown, Terrain und Unterlagen, Animationen, sämtliche Empfänger/Projektile, Randersatz, Dünger, Pause, Tod, Versetzen, echtes Speichern/Laden und der vollständige Almanach auf Deutsch/Englisch zu prüfen.

Installationsbeleg: `references/release-2.7.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.6.0.dll`; vorherige Quellen `backup/support-pots-2.6.0.tar.gz`, vorherige Offline-Suites `backup/tests-support-pots-2.6.0.tar.gz`. Anschließend einen Spielstand ohne neue Pflanzen- und Projektilkennungen laden.

## Änderung 2.6.0: Vier reaktive Topffusionen

Vier Ergebnisse aus der lokalen Fusion-Quelle 4.0.5.2 sind umgesetzt. Beide Zutaten waren bereits nativ verfügbar; jedes Ergebnis besitzt genau dieses eine Quellenrezept:

- Flower Pot + Cherry Bomb = **Cherry Pot** (Kirschtopf): Kirschexplosionen heilen alle Pflanzen desselben Feldes um 20 HP. Kirschschützen erhalten fünf Sekunden lang 50 % mehr Angriffsgeschwindigkeit. Weitere Explosionen erneuern die Dauer ohne Stapelung.
- Flower Pot + Jalapeno = **Hot Pot** (Feuertopf): Jalapeno-Reiheneffekte heilen Feuerschützen darüber um 20 HP und verleihen fünf Sekunden lang 50 % mehr Angriffsgeschwindigkeit. Zwischen Auslösungen liegen mindestens 0,2 Sekunden. Boni stapeln sich additiv und laufen unabhängig aus.
- Flower Pot + Hypno-shroom = **Hypno Pot** (Hypnotopf): Vorbeikommende hypnotisierte Zombies erhalten dauerhaft ein Drittel weniger numerischen Schaden. Im Radius einer Quellenwelteinheit und derselben Reihe heilt der Topf alle 0,04 Sekunden einen Körper-HP. Beim Biss hypnotisiert er den Angreifer und wird zu einem normalen Blumentopf; eine obere Pflanze bleibt erhalten.
- Flower Pot + Doom-shroom = **Doom Pot** (Untergangstopf): verhindert Untergangskrater des eigenen Feldes. Das kostet 75 maximale und aktuelle HP sowie anschließend 20 % der verringerten maximalen HP. Die Verhinderung gilt auch bei tödlichem Opfer. Krater erholen sich bei vorhandenem Topf doppelt so schnell, also in 90 statt 180 Sekunden, und heilen jede Sekunde den Untergangstopf mit den wenigsten HP derselben Reihe um 2 % seiner maximalen HP.

Alle vier Töpfe beginnen mit 300 HP und dem vorhandenen Erzeugungsschutz von 0,5 Sekunden. Dünger heilt ohne Entwicklung. Beim Untergangstopf stellt er geopferte maximale HP nicht wieder her. Originalmodelle, sämtliche Originalclips und Porträts sind eingebunden; der Feuertopf besitzt zusätzlich seine kontinuierliche additive Feueranimation. Es werden keine neuen Projektile benötigt. Verstärkte Schützen verwenden alle vorhandenen Projektile und Fähigkeiten. Native Repetierer-, Spalterbsen- und Gatling-Salven behalten ihre Ereignisse bei Restzähler 25 beziehungsweise 50.

Der Kirschtopf unterstützt die fünf vorhandenen Kirschschützenvarianten, den Kirschwerfer und den Knallpilz. Der Feuertopf unterstützt die vier Glutschützen, den versengten Dreifachschützen, den Flammenpilz und den Flammen-Düsterpilz. Noch nicht portierte Quellenpflanzen werden dadurch nicht als verfügbare Zutaten oder Empfänger ausgegeben. Angriffsboni verändern nur den Angriffsrestzähler; Schussanimationen und Projektilschäden bleiben bei der jeweiligen Familie. Hypnoseschutz betrifft numerischen Schaden; direkte Sonderentfernung ohne Schadenswert bleibt tödlich. Bruchteile erhalten die Wirkung bei kleinen Bissen.

Zeitlich begrenzte Boni und permanenter Hypnoseschutz werden als unsichtbare native GridItems mit vollständiger Empfänger-DataID gespeichert. Sie folgen dem Empfänger beim Versetzen und überleben die Entfernung des spendenden Topfes. Tod oder Pool-Wiederverwendung entfernt verwaiste Effekte. Der Krater-Heilzähler verwendet ein separates gespeichertes Feld; die bestehende Eisdauer des Demise-shroom-Kraters bleibt erhalten. Das native Doom-shroom besitzt eine eingebettete Stapelzerstörung vor der Kratererzeugung. Der neue Schutz übernimmt deshalb gezielt diesen Zweig, bevor andere Pflanzen verloren gehen; eigene Doom-Fusionen verwenden denselben Opfervertrag vor ihrer gemeinsamen Kratererzeugung.

Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die vorhandene Rezeptauflösung, Platzierungsprüfung und Erzeugung vor dem Zutatenverbrauch. Obere Pflanzen und Kürbisse bleiben erhalten. Alle vier erledigten Rezeptzeilen sind aus `fusionen.txt` entfernt; das Quellenarchiv bleibt unverändert. Offen bleiben 455 Rezepte mit 385 Ergebnissen. Der zweisprachige Fusionsalmanach umfasst 165 Ergebnisse. Die mittlere Versionsstelle steigt für vier neue Pflanzen auf **2.6.0**.

Fachliche Quellen, Verhalten, Speicherung, Anpassungen und konkrete Benutzerprüfungen stehen unter [Kirschtopf](references/cherry-pot/port-notes.md), [Feuertopf](references/hot-pot/port-notes.md), [Hypnotopf](references/hypno-pot/port-notes.md) und [Untergangstopf](references/doom-pot/port-notes.md). Statische Konsistenz, Originalexport, Kalibrierung und C#-Kompilierung sind geprüft. Alle 26 Adapter mit gemeinsamer Fusionsidentität werden ausschließlich kompiliert; Protokolle: `references/reactive-pots-validation/`. Die erweiterte Suite `DefensivePotsTests` enthält die tatsächliche neue Verhaltensklasse. Übrige Familienadapter isolieren sie mit einem gemeinsamen Kollaboratordouble.

**Spiel und sämtliche Testprogramme bleiben beim Benutzer.** Prüfen: alle Rezept- und Handschuhrichtungen, Dach, vorhandene obere Pflanzen/Kürbisse, Dünger, Originalanimationen, Angriffsschübe samt vollständigen Salven, separate Bonusabläufe und Dauererneuerung, Hypnoseheilung und dauerhafter Schutz, Bissumwandlung, tödliches Gesundheitsopfer, Kraterheilung und Reparaturzeit, Demise-Eisdauer, Pause sowie echtes Speichern/Laden während aller Effekte. Kommandos: `../.tools/dotnet/dotnet run --project tests/DefensivePotsTests.csproj -c Release` und die weiteren im Adapterbericht genannten Suites. Builds ersetzen diese Prüfungen nicht.

Installationsbeleg: `references/release-2.6.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.5.0.dll`; vorherige Quellen und Dokumentation: `backup/reactive-pots-2.5.0.tar.gz`. Für einen Rückbau einen Spielstand ohne neue Topfkennungen und ohne gespeicherte neue Effekte verwenden. Workflow, vollständiges Rezeptarchiv, Profile und Spielstände bleiben unverändert. Eine notwendige Workflowänderung mit belegtem Zeitgewinn entstand nicht.

## Änderung 2.5.0: Vier weitere Topffusionen

Vier Ergebnisse mit sämtlichen verfügbaren Quellenrezepten sind umgesetzt. Jedes hat genau ein Rezept; beide Zutaten sind bereits nativ verfügbar:

- Flower Pot + Wall-nut = **Nut Pot** (Nusstopf): 20 % weniger numerischer Kampfschaden in der eigenen Zelle, einschließlich gespeicherter Schadensbruchteile.
- Flower Pot + Garlic = **Garlic Pot** (Knoblauchtopf): genau 10 HP je erfolgreichem Biss an Knoblauchpflanzen derselben Zelle; der Beißer wechselt möglichst zur mittleren Reihe. Unterstützt Knoblauch, Silberknoblauch, Nusslauch, Faulpilz, Tripli-Stinker und die vier Gifterbsenvarianten. Hypnoknoblauch behält seine Umwandlung und Hypnose mit derselben Reihenpräferenz.
- Flower Pot + Spikeweed = **Spike Pot** (Stacheltopf): 20 Schaden an nahen Bodenzombies derselben Reihe ungefähr jede Sekunde, Reifenplatzen mit Topfverbrauch und 50 % weniger numerischer Schaden an der Stachelnuss darüber. Deren bestehender Fahrzeugkontakt kostet damit 250 statt 500 HP. Beißende Zombies ignorieren die niedrige Unterlage.
- Flower Pot + Melon-pult = **Melon Pot** (Melonentopf): einmalig 150 Sonne bei erfolgreicher Fusion. Melonapult, Wintermelone und Silbermelone darüber erhalten zwei Drittel Nebenzielschaden ohne Mengenabschwächung (53 bzw. 66). Direktschaden und Winterkälte bleiben erhalten. Bereits fliegende Geschosse behalten ihren gespeicherten Bonus nach Topfbewegung oder -tod.

Alle vier Töpfe haben 300 HP, native Topfplatzierung und den vorhandenen Erzeugungsschutz von 0,5 Sekunden. Dünger heilt die vorhandenen Schichten; keine der vier Quellen-IDs besitzt eine Düngerentwicklung. Der Stacheltopf erhält keine zusätzliche Dünger-Nachladung. Originalmodelle, sämtliche benötigten Clips und Porträts stammen aus der lokalen 4.0.5.2-Quelle. Der zweisprachige Almanach umfasst 161 Ergebnisse. Kalibrierungsblätter dokumentieren feste Größen und Anker; sie ersetzen keine Sichtprüfung im Spiel.

Native Sofortvernichtung und Zerquetschen ohne numerischen Schaden bleiben tödlich. Replanted besitzt keine Lavafelder für die Lavaimmunität des Nusstopfes. Die zusätzlichen Synergien mit der nicht verfügbaren Obsidian-Stachelnuss, Chronomelone, Maelonstorm, Unheil-Melonapult und Melonenhülle bleiben inaktiv. Die Melonen-Zwille hat keinen verstärkbaren Flächenschadenspfad. Fähigkeiten, Quellenadressen, Speicherung und konkrete Benutzerprüfungen stehen unter [Nusstopf](references/nut-pot/port-notes.md), [Knoblauchtopf](references/garlic-pot/port-notes.md), [Stacheltopf](references/spike-pot/port-notes.md) und [Melonentopf](references/melon-pot/port-notes.md).

Version 2.5.0 erhöht die mittlere Stelle für neue Pflanzen. Statische Konsistenz, Originalexport, Kalibrierung und C#-Kompilierung werden geprüft; Spiel und sämtliche Testprogramme bleiben ungestartet. Buildprotokolle: `references/defensive-pots-validation/`; Installationsbeleg: `references/release-2.5.0.json`. Der Benutzer prüft insbesondere beide Rezept- und Handschuhrichtungen, Terrain, Dünger, tödliche Grenztreffer, Reihenwechsel, Stachelangriff und Reifenplatzen, verstärkte Melonen sowie echtes Speichern/Laden. Die vorbereiteten Suites `DefensivePotsTests`, `ShroomSlotsTests`, `SunflowerFusionsTests`, `PuffLobbersTests`, `WeedFusionsTests` und `SupportFusionsTests` können mit `../.tools/dotnet/dotnet run --project tests/<Name>.csproj -c Release` ausgeführt werden.

Vorherige Quellen und Dokumentation: `backup/defensive-pots-2.4.0.tar.gz`. Vorherige installierte DLL: `backup/FusionMod-2.4.0.dll`. Für einen Rückbau einen Spielstand ohne neue Topfkennungen verwenden. Workflow, vollständiges Rezeptarchiv, Profile und Spielstände bleiben unverändert.

## Änderung 2.4.0: Drei Topffusionen und natives Maiskanonenrezept

Vier weitere Ergebnisse aus `fusionen.txt` sind umgesetzt. Alle Zutaten waren bereits vollständig verfügbar; jedes Ergebnis hat genau ein Quellenrezept:

- Flower Pot + Cabbage-pult = **Cabbage Pot** (Kohltopf): verkürzt die Angriffsintervalle von Kohlapult, Salat-Zwille und Silberkohl um ein Drittel, also ungefähr 50 % mehr Angriffe.
- Flower Pot + Kernel-pult = **Butter Pot** (Buttertopf): erhöht die Butterchance von Kornapult, Mais-Zwille und Silberkornapult von 25 auf 50 %. Butteranimation, Schrumpfen, Schaden, Betäubung, Geld und gespeicherte Geschosse bleiben bei ihren vorhandenen Familien.
- Flower Pot + Umbrella Leaf = **Umbrella Pot** (Schirmtopf): erhöht den Rückstoß von Schirmnuss, Schirmhülle und Silberschirm von 160 auf 240 Boardpixel. Fingerhut-Schirm erhält ebenfalls 50 % mehr Rückstoß (etwa 118,52 auf 177,78 Boardpixel nach dessen bestehendem Kleinpflanzenmaßstab). Schirmhülle betäubt mit Butter vier statt zwei Sekunden. Der Topf selbst wehrt keine Basketbälle oder Bungees ab.
- Kernel-pult + Cob Cannon = **Cob Cannon** (Maiskanone): ausschließlich Rezept für die vorhandene native Pflanze. Eine Kanonenkarte benötigt weiterhin zwei benachbarte Kornapulte. Kornapult auf vorhandener Kanone ersetzt diese innerhalb ihres Zweifeldplatzes. Beim Handschuh gelten dieselben Anforderungen. Es entsteht keine zusätzliche Maiskanonenimplementierung.

Beim Quellenabgleich wurde ein vertauschter Nullvergleich in der bisherigen Schirmportierung gefunden. Schirmnuss, Schirmhülle und Silberschirm stoßen ohne Topf korrekt zwei statt der bisher verwendeten drei Quelleinheiten zurück. Mit Schirmtopf gelten drei Einheiten. Der tatsächlich aufgerufene Gleichheitshelfer und die PE-Konstanten sind unter `references/support-pot-effects/` archiviert. Der kurze Fingerhut-Schirm hatte bereits den richtigen Grundwert; er erhält jetzt den belegten Bonus. Nicht portierte Kohl-, Knoblauch- und Reflexionsschirme sowie Superfähigkeiten bleiben ausdrücklich nicht verfügbar.

Alle drei Töpfe besitzen 300 Lebenspunkte und den gemeinsamen gespeicherten Erzeugungsschutz von 0,5 Sekunden. Originalmodelle, sämtliche Leerlaufclips, Sprites, Porträts und vollständige englische/deutsche Almanacheinträge sind eingebunden. Die Töpfe erzeugen keine eigenen Projektile. Native und vorhandene importierte Projektile werden vollständig wiederverwendet. Dünger heilt die Töpfe und die native Kanone; keine dieser Ergebnis-IDs erhält eine neue Entwicklung, Zusatzsalve oder sofortige Nachladung. Die nur in der anderen Quellfassung vorhandene Düngerentwicklung zweier Kornapulte wird nicht zusätzlich eingeführt, da die Maiskanone ausdrücklich nativ bleibt.

Topfeffekte werden aus den lebenden Pflanzen derselben Zelle bestimmt. Versetzen, Entfernen und Laden hinterlassen keinen dauerhaften Bonus. Der Kohltopf verändert nur neu angesetzte Intervalle; ein bereits laufender Restzähler bleibt bestehen. Butterwahl und passende Animation werden einmal pro Angriff gespeichert. Karten und Handschuh erzeugen Ergebnisse vor dem Zutatenverbrauch. Der Maiskanonenweg schützt und verbraucht beide Kornapulte gemeinsam; beide Ergebnisfelder werden geprüft. Bereits vorhandene Pflanzen oberhalb einer Topfzutat und Kürbisse bleiben erhalten. Die Kanonenfusion initialisiert Gesundheit und Nachladephase neu; der Zustand einer verbrauchten Kanone wird nicht übertragen.

Fachliche Quellen, Speicherung, Anpassungen und Benutzerprüfungen stehen unter `references/cabbage-pot/port-notes.md`, `references/butter-pot/port-notes.md`, `references/umbrella-pot/port-notes.md` und `references/native-cob-cannon/port-notes.md`. Die vier erledigten Zeilen sind entfernt; das Quellenarchiv bleibt unverändert. Offen bleiben 463 Rezepte mit 393 Ergebnissen. Der Fusionsalmanach umfasst 157 Ergebnisse. Für drei neue Pflanzen steigt die mittlere Versionsstelle auf **2.4.0**. Workflow und seine Verknüpfung bleiben unverändert; eine notwendige Ablaufänderung mit gemessenem Zeitgewinn ist nicht entstanden.

Statische Konsistenz, Originalexport, Kalibrierung und Release-Build sind geprüft. Alle 25 betroffenen C#-Adapter wurden ausschließlich kompiliert; Protokoll `references/support-pots-adapter-builds-2.4.0.txt`. Spiel und sämtliche Testprogramme bleiben beim Benutzer. Installationsbeleg: `references/release-2.4.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.3.0.dll`; vorherige Quellen und Dokumentation: `backup/support-pots-2.3.0.tar.gz`. Neue Topfvarianten vor dauerhaftem Rückbau aus weiterzuverwendenden Spielständen entfernen.

Manuell prüfen: alle vier Rezepte in beiden Karten-/Handschuhrichtungen, Töpfe unter vorhandenen Pflanzen, Dach und Kürbisse, tatsächliche Wurfintervalle, Butterwahl samt Animation und bereits fliegenden Geschossen, Grundrückstoß und Topfbonus aller vier Schirmfamilien, vier Sekunden Butter und Immunitäten, Erzeugungsschutz und Dünger, Pause, Versetzen/Entfernen, Speichern/Laden und beide Almanachsprachen. Bei der Kanone insbesondere zweiten Kornapult, Randspalte, belegtes rechtes Feld, Erzeugungsfehler ohne Zutatenverlust, Nachladen und manuelles Abfeuern prüfen. Vorbereitete Regressionen: `PuffLobbersTests`, `PuffSupportTests`, `UmbrellaShellTests`, `SunflowerFusionsTests` und `ShroomSlotsTests`. Builds ersetzen diese Prüfungen nicht.

## Änderung 2.3.0: Silbertopf, Silberschirm, Silberkornapult und Lichttopf

Vier weitere Ergebnisse aus der lokalen Fusion-Quelle 4.0.5.2 sind umgesetzt:

- Flower Pot + Marigold = **Silver Pot**: 300 Lebenspunkte, Unterlage für eine weitere Pflanze, Geldproduktion etwa alle 25 Sekunden.
- Marigold + Umbrella Leaf = **Silver Umbrella**: 1000 Lebenspunkte, nativer Schutz im Bereich von 3 × 3 Feldern, 40 Kontaktschaden mit Rückstoß und 25 Münzen je abgewehrtem Basketball. Kontaktangriffe kosten bei dieser Quellvariante keine Lebenspunkte und bringen kein Geld; Bungees bringen ebenfalls kein Geld.
- Kernel-pult + Marigold = **Silver Kernel**: 300 Lebenspunkte, Wurf etwa alle drei Sekunden, 40 Körnerschaden. Mit 25 % Wahrscheinlichkeit wirft die Pflanze Silberbutter für 60 Schaden, vier Sekunden native Betäubung und 25 Münzen. Eigene Reihe hat Vorrang; ohne geeignetes Ziel dort werden andere Reihen berücksichtigt. Butter zahlt auch bei Bodentreffern.
- Flower Pot + Plantern = **Lumos Pot**: 300 Lebenspunkte, Unterlage mit Licht für genau das eigene Feld. Die native Nebelmaske wird an den beiden vertikalen Feldkanten aufgehellt; es entsteht kein zusätzliches System für Fusion-Lichtstufen.

Alle Zutaten waren bereits vollständig verfügbar. Jedes Ergebnis hat genau dieses eine Quellenrezept. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die bestehenden Wege; das belegt keine Spieltests. Topffusionen erhalten bereits vorhandene Pflanzen oberhalb der Unterlage. Kosten und Karten-Cooldown stammen weiterhin von der eingehenden Zutat. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; das vollständige Quellenarchiv bleibt unverändert.

Originalpflanzen, beide Silbermaisprojektile, Animationen, Ereignisbelege und vollständige englische/deutsche Almanacheinträge sind eingebunden. Silbermais verwendet die Originalereignisse bei 0,333333 Sekunden; die gespeicherte Butterwahl wählt auch den passenden `shoot2`-Clip. Native Schatten, Flugbewegung und Speichern bleiben im bestehenden Renderer und Projektilformat. Die Kalibrierung zeigt den hoch ausschwingenden Wurfarm teilweise außerhalb des festen Vergleichsbildes; das ist eine Bildbegrenzung der Offlinevorschau und keine beschnittene Laufzeitgrafik.

Replanted speichert Geld in Zehnerschritten. Die neuen 25er-Erträge zahlen deshalb abwechselnd 20 und 30 Münzen als native Silbermünzen aus. Der Rest von fünf Münzen bleibt in `Plant.mTargetY` gespeichert, bei Silberbutter steht der bereits bestimmte Auszahlungswert zusätzlich im gespeicherten Projektilfeld `mCobTargetRow`. Ein einzelner Ertrag ist damit nicht exakt 25; nach zwei Erträgen stimmt die Summe. Stirbt eine Pflanze mit Rest, verfällt dieser Rest. Bereits erzeugte Münzen und fliegende Butter bleiben nativ gespeichert. Es gibt keine direkten Profil- oder Inventarschreibzugriffe; erst das normale Einsammeln der Münzen verwendet den nativen Geldweg.

Dünger heilt alle vier Pflanzen auf ihre maximale Gesundheit. Keine dieser Quellen-IDs besitzt eine Düngerentwicklung. Beide neuen Töpfe erhalten den gemeinsamen 0,5-Sekunden-Schutz der Quelle. Er blockiert Bisse, Zerquetschen, feindliche Projektile und Explosionen beziehungsweise feindliches Reihenfeuer. Der Restzeitraum wird gespeichert; Dünger verkürzt ihn nicht weiter. Schaufel, Zutatenverbrauch und Board-Abbau bleiben möglich. Dünger löst deshalb weder Geldproduktion noch einen zusätzlichen Angriff aus. Die separat bezahlten Goldpflanzen und Superfähigkeiten gehören nicht zu diesen vier normalen Rezeptresultaten und sind nicht aktiviert. Die aktuellen Quellmethoden ersetzen hierbei veraltete englische Angaben über einen Lebenspunkteverlust beim Kontakt des Silberschirms.

Fachliche Belege und konkrete Benutzerprüfungen: `references/silver-pot/port-notes.md`, `references/silver-umbrella/port-notes.md`, `references/silver-kernel/port-notes.md`, `references/lumos-pot/port-notes.md`. Der aufwendigste Teil war der Abgleich der Originalfähigkeiten mit den nativen Aufrufern und Speicherfeldern, besonders Silbermais-Zielsuche, Buttertreffer, Basketballabwehr und kurzzeitiger Topfschutz. Es liegen keine getrennten Zeitmessungen für alle Arbeitsschritte vor. Der Workflow bleibt unverändert, weil keine notwendige Ablaufänderung mit belegtem Zeitgewinn entstanden ist.

Die mittlere Versionsstelle steigt für vier neue Pflanzen auf **2.3.0**. Statische Konsistenz, Originalexport und Release-Build werden geprüft; zehn betroffene C#-Testprojekte wurden ausschließlich kompiliert. Spiel und sämtliche Testprogramme bleiben beim Benutzer. Buildprotokoll: `references/silver-support-adapter-builds-2.3.0.txt`; Installation: `references/release-2.3.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.2.0.dll`. Vorherige Quellen und Dokumentation: `backup/silver-support-2.2.0.tar.gz`. Neue Varianten vor einem dauerhaften Rückbau aus weiterzuverwendenden Spielständen entfernen.

Manuell prüfen: alle vier Rezepte in beiden Richtungen mit Karten und Handschuh, besetzte Töpfe und Dach, sichtbare Originalanimationen, Reihenwechsel der Silbermaisziele, Butter auf Einzelziel und Boden, vier Sekunden Betäubung und Immunitäten, Basketballabwehr gegenüber Bungee-/Kontaktangriffen, Geldsummen über mehrere Erträge, Lichtfeld und erneutes Zuziehen des Nebels nach Entfernung, Dünger, Pause, Tod und Speichern/Laden während Wurf, Flug und Produktion. Bei beiden Töpfen außerdem Treffer unmittelbar nach Erzeugung und nach Ablauf der ersten halben Sekunde prüfen. Englischen und deutschen Almanach sowie Sprachwechsel prüfen. Passende vorbereitete Tests stehen in `PuffLobbersTests`, `SunflowerFusionsTests` und `UmbrellaShellTests`; die vollständige Adapterliste steht im Buildprotokoll.

## Änderung 2.2.0: Vier weitere Silberfusionen

Neu sind Doom-shroom + Marigold = Silver Doom-shroom, Hypno-shroom + Marigold = Silver Hypno-shroom, Cabbage-pult + Marigold = Silver Cabbage und Marigold + Melon-pult = Silver Melon. Alle Zutaten waren bereits verfügbar; jedes Ergebnis hat genau dieses Quellenrezept. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden den bestehenden Fusionsweg. Die vier abgeschlossenen Rezeptzeilen sind aus `fusionen.txt` entfernt.

Silver Doom-shroom bleibt gemäß Original vollständig stabil: keine Explosion, kein Krater und keine Münzproduktion. Zerschmettern entfernt ihn ohne Explosion. Silver Hypno-shroom verwendet die native Hypnose beim Fressen und lässt nach erfolgreicher Hypnose insgesamt 100 Geld fallen. Zwei native Goldmünzen zu je 50 erhalten den Geldwert auch beim Speichern und Laden. Beide Pilze behalten Replanteds Schlaf- und Kaffeeregeln.

Silver Cabbage wirft silberne Kohlköpfe mit 60 Direktschaden, Silver Melon silberne Melonen mit 100 Direktschaden und bis zu 33 Flächenschaden. Beide greifen etwa alle drei Sekunden an; ihre Originalclips lösen den Kohlwurf nach 0,316667 und den Melonenwurf nach 0,333333 Sekunden aus. Die native ballistische Bewegung übernimmt die 1,5 Sekunden lange Flugbahn. Melonen verursachen auch bei Bodentreffern Flächenschaden innerhalb von 1,5 Quellenwelteinheiten und höchstens einer Nachbarreihe. Bei sehr großen Gruppen sinkt dieser Schaden nach der originalen Mengenbegrenzung auf mindestens 1. Getragene Schilde werden überworfen. Silberkohlgeschosse drehen sich mit 300 Grad pro Sekunde.

Alle vier Pflanzen haben 300 Lebenspunkte. Dünger heilt sie; er aktiviert keine Explosion, keine zweite Münzbelohnung und kein Goldpflanzen-Upgrade. Die separat gegen Geld erworbenen Goldpflanzen und ihre zusätzlichen Fähigkeiten sind eigenständige, weiterhin nicht verfügbare Ergebnisse. Originalmodelle, sämtliche Clips, beide Projektilmodelle und die Porträts stammen aus der lokalen 4.0.5.2-Quelle. Der zweisprachige Fusionsalmanach umfasst jetzt 149 Ergebnisse. Größen und Anker sind in statischen Kalibrierungsblättern dokumentiert.

Fähigkeiten, Quellen, Speicherung, Abweichungen und konkrete Benutzerprüfungen stehen unter `references/silver-doom-shroom/port-notes.md`, `references/silver-hypno-shroom/port-notes.md`, `references/silver-cabbage/port-notes.md` und `references/silver-melon/port-notes.md`. Die Versionsnummer steigt für vier neue Pflanzen auf 2.2.0. Statische Konsistenz und Release-Build werden geprüft; Spiel und sämtliche Testprogramme bleiben geschlossen. Die erweiterten vorhandenen C#-Testprojekte sind nur kompiliert. Releasebeleg: `references/release-2.2.0.json`. Rückbau der DLL bei geschlossenem Spiel: `backup/FusionMod-2.1.6.dll`. Vorherige Quellen und Dokumentation: `backup/silver-combat-2.1.6.tar.gz`. Workflow, vollständiges Rezeptarchiv, Profile und Spielstände bleiben unverändert.

## Änderung 2.1.6: Fachliche Dokumentation der Fähigkeiten und Grenzen

Die neue [Übersicht zu Fähigkeiten und Grenzen](faehigkeiten-und-grenzen.md) beschreibt gemeinsame Fusionsregeln, Rezeptstatus, Speicherung und die Aussagekraft der Portierungswerkzeuge. Ein vorhandener Cherry-spreader-Port zeigt die Trennung von Fähigkeit, Zahlen mit Einheiten, Anpassung und Grenze. [Workflow, Abschnitt 7.2](workflow.md#72-fachliche-dokumentation-der-fähigkeiten-und-grenzen), auch über `worklfow.md` erreichbar, legt die fachlichen Notizen pro Ergebnis fest: Originalbelege, Replanted-Umsetzung, fehlende Fähigkeiten, Lebenszyklus und konkrete Benutzerprüfungen.

`python tools/fusion_checks.py notes --result "Cherry-spreader"` erzeugt einen ausdrücklich ungeprüften Markdown-Entwurf nach stdout. Der bestehende Helfer übernimmt Quellen-ID, bestätigte Ergebnisidentität, hinterlegten Verfügbarkeitsbeleg und alle Rezeptalternativen aus den vorhandenen Projektdateien. Aktive, gesperrte und noch unregistrierte Rezepte bleiben getrennt. Eingabeprüfsummen halten den Stand fest; widersprüchliche Identitäten, inkonsistente Registrierungen oder während der Erzeugung geänderte Eingaben führen zu einem Fehler. Fähigkeiten, native Aufrufer, Grafik und Speicherverhalten werden weiterhin fachlich belegt. Bestehende Notizen werden nicht überschrieben. Es gibt keine neue Datenpflege für Fähigkeiten und keinen erneuten Quellenexport. Ein Zeitgewinn für die gesamte Portierung wurde nicht gemessen.

Für den beauftragten Release steigt die Fixnummer auf 2.1.6. Gameplay, Pflanzen, Rezepte, Speicherkennungen und Grafikassets ändern sich nicht. Python-Syntax, tatsächliche Dokumentationsausgaben, statische Konsistenz und Release-Build werden geprüft; Regressionstests und Spieltests bleiben beim Benutzer. Ergänzte synthetische Prüfungen stehen in `tools/test_workflow.py`. Installation und Buildbeleg: `references/release-2.1.6.json`. Rückbau der Mod bei geschlossenem Spiel: `backup/FusionMod-2.1.5.dll`. Die vorherigen Werkzeug-, Versions- und Dokumentationsdateien liegen unter `backup/capability-docs-2.1.5.tar.gz`.

## Änderung 2.1.5: Gezielter Erstexport ohne Vollindex

Der Grafikexport lädt das Quellenbundle weiterhin einmal pro Batch, baut dafür jedoch keinen vollständigen Quellenindex mehr auf. Er prüft die gewählten Prefabs, Root-Transforms und AnimatorController direkt. Dadurch entfällt beim ersten gezielten Export ohne Cache das zusätzliche Dekodieren sämtlicher GameObjects, Controller und AnimationClips für die globale Namenssuche. Der Quellenindex bleibt für die Recherche verfügbar. Spritebild und Metadaten stammen jetzt aus einem einmal gelesenen UnityPy-Objekt. Streamed-Frames verwenden die Stapeldekodierung der Standardbibliothek; ungültige Framegrößen und Kanalindizes werden abgewiesen. PNG-Kompression und Modellformat bleiben erhalten.

Der CLI-Export erzeugt und prüft den gesamten ausgewählten Batch zunächst in einem temporären Projektordner. Die unterstützte Quellen-DLL wird vorab geprüft; Eingabeprüfsummen werden vor der Übernahme erneut kontrolliert. Anschließend ersetzen Umbenennungen auf demselben Dateisystem die geänderten Dateien, während bytegleiche Ziele erhalten bleiben. Erfolgsbelege werden zuletzt übernommen. Ein Fehler beim Dekodieren oder eine geänderte Quelle lässt die bisherigen Ausgaben unverändert. Scheitert die abschließende Übernahme, können Dateien bereits ersetzt sein; fehlende Belege verlangen einen erneuten Export. Eine Dateisperre weist gleichzeitige CLI-Exporte ab. Der temporäre Ordner benötigt Platz für den gewählten Batch. Laufzeiten für Laden, Dekodieren mit Ausgabe und Übernahme erscheinen getrennt; ein gemessener Vergleich oder Gesamtzeitgewinn liegt nicht vor.

Anleitung und Grenzen stehen in `workflow.md`, Abschnitt 5, auch über `worklfow.md` erreichbar. Der vorhandene Aufruf mit wiederholtem `--plant`, `--all` und das Standardmodell bleiben verfügbar. Für den beauftragten Release steigt die Fixnummer auf 2.1.5. Gameplay, Pflanzen, Rezepte, Speicherkennungen und vorhandene Grafikassets ändern sich nicht. Python-Syntax, statische Konsistenz und Release-Build werden geprüft. Spiel, Exportläufe und Regressionstests bleiben beim Benutzer; die ergänzten synthetischen Prüfungen stehen in `tools/test_export_snow_repeater.py`. Installation und Buildbeleg: `references/release-2.1.5.json`. Rückbau der Mod bei geschlossenem Spiel: `backup/FusionMod-2.1.4.dll`. Die bisherigen Werkzeug- und Dokumentationsdateien liegen unter `backup/source-export-2.1.4.tar.gz`.

## Änderung 2.1.4: Gemeinsame C#-Testadapter

Die Offline-Testprojekte teilen ihre zusammengehörigen Modquellen jetzt über fünf MSBuild-Quellgruppen unter `tests/Adapters/`. Eine zusätzliche Gruppe bindet die echte Fusionsidentität und eine vollständige native `SeedType`-Definition ein; 16 bisher getrennte Enumkopien entfallen. Die bisherigen Standardidentitäten der Testdoubles sind ausdrücklich gesetzt. Unterschiedliche Verhaltensdoubles bleiben bei ihren jeweiligen Familien. Eine neue gemeinsame Abhängigkeit wird in der passenden Gruppe einmal ergänzt, statt in jedem Verbraucher einzeln.

`python tools/csharp_adapters.py --changed src/ShroomSlots.cs` zeigt alle betroffenen Suites und deren manuelle Testbefehle. Das Skript fragt die tatsächlich von MSBuild ausgewerteten Quellen ab und erkennt gemeinsame Stub-Dateien, importierte Quellgruppen sowie zentrale Projekteinstellungen. Fehlende oder doppelte Eingaben und Dateien ohne angebundene Suite erzeugen einen Fehler. Ohne Dateiauswahl erscheinen alle 27 Projekte. `--build` kompiliert die ausgewählten Projekte ausschließlich; Testprogramme werden niemals gestartet. Anleitung und Grenzen stehen in `workflow.md`, Abschnitt 7.1, auch über `worklfow.md` erreichbar.

Für den beauftragten Release steigt die letzte Versionsstelle auf 2.1.4. Gameplay, Pflanzen, Rezepte und Speicherkennungen ändern sich nicht. Alle 27 Offlineprojekte werden nur gebaut; Regressionstests und Spieltests bleiben beim Benutzer. Die neuen synthetischen Werkzeugprüfungen liegen unter `tools/test_csharp_adapters.py` und wurden nicht ausgeführt. Statische Migrationsbelege und Buildausgaben liegen unter `references/csharp-adapters-*-2.1.4.*`; der Releasebeleg liegt unter `references/release-2.1.4.json`. Rückbau der Mod bei geschlossenem Spiel: `backup/FusionMod-2.1.3.dll`. Die bisherigen Test-, Werkzeug- und Dokumentationsdateien liegen unter `backup/csharp-adapters-2.1.3.tar.gz`. Eine Beschleunigung des gesamten Portierens wurde nicht gemessen.

## Änderung 2.1.3: Gemeinsame Grafikabstimmung

`tools/preview_snow_repeater.py --calibrate` ergänzt die vorhandene Originalvorschau um ein Vergleichsblatt mit festem Maßstab und einen JSON-Messbericht. Pflanzengröße und Bodenanker werden einmal aus den echten Spritegrenzen der initialen Pose berechnet. Ein gemeinsames Raster macht Größenunterschiede und Animationsversatz sichtbar. Optionale Projektile verwenden denselben Maßstab, eine ausdrücklich belegte Referenzbreite und bei Bedarf den Faktor beziehungsweise die Root-Korrektur ihres Laufzeitadapters. Bekannte Puff-Varianten benötigen die vollständige Grafikmessung und eine gemessene native Zielbreite. Der Bericht enthält Grenzen, Knotenursprünge, Skalierung, Anker sowie Eingabe- und Bildprüfsummen.

Die vorhandenen Animations-, PNG- und Bindungscaches bleiben gemeinsam genutzt. Die Vorschau berücksichtigt jetzt auch bei außermittigen Pivots die Spiegelung um den Spriteursprung; Nullskalierungen zeichnen keine Fläche. Die bisherige Originalvorschau und der spezielle Sonnenkugelvergleich bleiben verfügbar. Anleitung und Beispiele stehen in `workflow.md`, Abschnitt 5.1, auch über `worklfow.md` erreichbar. Ein Zeitgewinn beim gesamten Portieren ist noch nicht gemessen. Die Vorschau ersetzt keine Prüfung der nativen Controller, Flugbahnen oder Fähigkeiten.

Für den beauftragten Release steigt die letzte Versionsstelle auf 2.1.3. Gameplay, Pflanzen, Rezepte und Speicherkennungen ändern sich nicht. Spiel, Regressionstests und die erste Vorschauerzeugung bleiben beim Benutzer; vorbereitet sind `tools/test_preview_snow_repeater.py`. Syntax, statische Konsistenz und Release-Build werden geprüft. Installation und Buildbeleg: `references/release-2.1.3.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.1.2.dll`. Die bisherigen Werkzeug- und Dokumentationsdateien liegen unter `backup/visual-calibration-2.1.2/`.

## Änderung 2.1.2: Gemeinsame native Verhaltensanalyse

`tools/research_native.py` ersetzt für neue Recherchen die wiederholten Replanted-Einmalskripte. Ein SHA-256-geprüfter Aufruferindex wird zwischen Ergebnissen wiederverwendet. Das Werkzeug archiviert direkte Aufrufe und Sprünge einschließlich `jmp`, nennt indirekte Aufrufe als offen und ergänzt Ziel- sowie Lebenszyklusmethoden als Suchkandidaten. Für ausgewählte native Felder sammelt es Lese-, Schreib- und Adresskandidaten einschließlich überdeckender Reset-Schreibzugriffe. Tokenbasierte Dateinamen erhalten Überladungen; Quellprüfungen, Ausgabeprüfsummen, Dateisperre und ein zuletzt geschriebener Erfolgsbeleg schützen die Rechercheausgaben.

Anleitung: `workflow.md`, Abschnitt 4.1, auch über `worklfow.md` erreichbar. Statischer Beispielbeleg: `references/native-analysis-potato/native-report.md`. Er zeigt unter anderem den tatsächlichen `jmp` von `Zombie.CheckIfPreyCaught` zu `Zombie.EatPlant` und Speicher-/Lebenszykluskandidaten für `Plant.mStateCountdown`. Methodenbereiche können weitere native Helfer enthalten; Offsettreffer belegen ohne Registeranalyse keinen Objekttyp. Virtuelle Aufrufe, fremde Feldschreiber, eingebettete Implementierungen und tatsächliche Fähigkeiten bleiben fachlich zu prüfen. Ein Gesamtzeitgewinn beim Portieren wurde nicht gemessen.

Für den ausdrücklich beauftragten Release steigt die letzte Versionsstelle auf 2.1.2. Gameplay, Pflanzen, Rezepte und Speicherkennungen ändern sich nicht. Die Regressionstests in `tools/test_research_native.py` sind vorbereitet und bleiben zusammen mit Spieltests beim Benutzer. Python-Syntax und tatsächliche statische Recherche werden geprüft; Spiel und Testprogramme werden nicht gestartet. Installation und Buildbeleg: `references/release-2.1.2.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.1.1.dll`.

## Änderung 2.1.1: C#-Registrierungen gemeinsam erzeugen

Der Portierungsworkflow pflegt Identität, `FusionPlant.From`, Familien und Pflanzenrenderer jetzt gemeinsam in `tools/plant-registrations.json`. `tools/registrations.py` erzeugt daraus einen prüfbaren Patch für die bisherigen C#-Dateien. Für eine neue importierte Pflanze genügen dort ihr Speicherpaar und die fachlich passenden Familienzuordnungen. Modellname, Wiedererkennung und `HasImportedVisuals` folgen automatisch. Verhalten, Rezepte, Quellenbelege, Exporte, Ressourcen, Zutatenverfügbarkeit und Almanach bleiben in ihren vorhandenen Verantwortlichkeiten.

135 importierte Pflanzen, 31 Familien und eine reservierte Synergie sind übernommen. Die 134 gespeicherten Variantenpaare, die von `From` wiedererkannten Paare, Familienüberschneidungen sowie Rendererreihenfolge und Ressourcennamen bleiben laut statischem Quellenvergleich unverändert. Gloom-shroom bleibt nativ; Starry Pumpkin bleibt reserviert. Unbekannte Marker und native Imitater behalten ihre native Identität. Es gibt keine neue Pflanze und keine beabsichtigte Gameplayänderung. Die Konfigurationsdatei wird ausschließlich für die Entwicklung benötigt; die Mod lädt weiterhin ausschließlich C#-Registrierungen.

Die bestehende Releaseprüfung verweigert veraltete erzeugte C#-Registrierungen. Anleitung und Benutzerprüfungen stehen in `workflow.md`, Abschnitt 6.1, auch erreichbar über `worklfow.md`. Quelldateien vor der Umstellung: `backup/csharp-registrations-2.1.0/`. Statischer Vergleich: `references/csharp-registrations/migration-2.1.1.json`. Neue Regressionstests sind in `tools/test_registrations.py` vorbereitet; die bestehende Workflowprüfung berücksichtigt veraltete Familien- und Renderereinträge. Tests und Spielstart bleiben beim Benutzer. Ein gemessener Zeitgewinn liegt nicht vor.

2.1.1 erhöht die letzte Versionsstelle für die Konsolidierung bestehender Registrierungen. Installation und Buildbeleg: `references/release-2.1.1.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.1.0.dll`.

## Änderung 2.1.0: Vier Silberfusionen

Neu sind Marigold + Wall-nut = Silver-nut, Garlic + Marigold = Silver Garlic, Ice-shroom + Marigold = Silver Ice-shroom und Marigold + Sunflower = Silver Sunflower. Alle Zutaten waren bereits nativ verfügbar. Jedes Ergebnis besitzt genau dieses Quellenrezept; beide Kartenreihenfolgen und Handschuhrichtungen werden unterstützt. Die vier erledigten Rezeptzeilen entfallen aus der offenen Liste.

Silver-nut hat 6000 Lebenspunkte, Silver Garlic 600 mit nativem Knoblauch-Reihenwechsel. Silver Ice-shroom aktiviert nach 1,5 Sekunden Wachzeit einen globalen Frost mit 20 Schaden und fünf Sekunden Einfrieren; Zerschmettern aktiviert ihn ebenfalls. Kaffee bleibt nativ verarbeitet. Silver Sunflower erzeugt 25 Sonne je etwa 15 Sekunden einschließlich originaler Streuung und Leuchtphase. Dünger heilt die Varianten; beim Silbereispilz verkürzt er einen längeren Rest auf 0,5 Sekunden. Keine dieser Pflanzen benötigt neue Angriffsprojektile. Separate Goldpflanzen-Upgrades gegen Münzen bleiben außerhalb dieser vier Silber-Ergebnisse und sind ausdrücklich im Almanach vermerkt.

Originalmodelle, sämtliche Originalclips, Schadensbilder und Originalporträts stammen aus der lokalen 4.0.5.2-Quelle. Der Fusionsalmanach umfasst jetzt 145 Ergebnisse mit vollständigen englischen und deutschen Texten. Quellen, gespeicherte Zustände, Fähigkeiten, Dünger, Grenzen und manuelle Prüfpunkte: `references/silver-plants/port-notes.md`. Workflow und vollständiges Rezeptarchiv bleiben unverändert.

2.1.0 erhöht die mittlere Versionsstelle für vier neue Pflanzen. Releaseprüfung und Build erfolgen über den bestehenden Releaseweg; Spiel und Testprogramme werden nicht gestartet. Installation: `references/release-2.1.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-2.0.0.dll`. Spieltests und vorbereitete Regressionstests übernimmt der Benutzer.

## Änderung 2.0.0: Vier Fusionsrezepte für native Upgradepflanzen

Neu sind Ice-shroom + Melon-pult = Winter Melon, Magnet-shroom + Marigold = Gold Magnet, Lily Pad + Cattail = Cattail und Spikeweed + Spikerock = Spikerock. Jedes Ergebnis hat genau dieses Quellenrezept. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden den vorhandenen Fusionsweg; alle Zutaten sind bereits nativ verfügbar. Die vier erledigten Einträge entfallen aus der offenen Liste.

Alle vier Pflanzen existieren bereits in Replanted und werden deshalb ausschließlich nativ erzeugt. Grafik, Animationen, Fähigkeiten, Projektile und Speicherung werden wiederverwendet. Gold Magnet sammelt nativ Münzen und Diamanten; die Projektilanziehung und Schusssteuerung aus PvZ Fusion werden nicht ergänzt. Winter Melon behält native Kälte und Flächenschaden ohne Fusion-Kryopunkte oder vierfachen Gefrierschaden. Dünger heilt die Ergebnisse ohne weitere Entwicklung. Die bisherigen Dünger-Upgrades von Spikeweed und freiliegender Lily Pad bleiben bestehen.

Der Fusionsalmanach enthält jetzt 141 Ergebnisse mit Originalporträts sowie vollständigen englischen und deutschen Einträgen. Quellen, native Identitäten, Verhalten, Abweichungen und manuelle Prüfpunkte: `references/native-upgrade-recipes/port-notes.md`. Workflow und vollständiges Quellenarchiv bleiben unverändert.

2.0.0 erhöht die Gameplaystelle für zusätzliche Rezepte ohne neue Pflanzenimplementierungen. Release-Build und statische Prüfung erfolgen über den bestehenden Releaseweg; Testprogramme und Spieltests bleiben beim Benutzer. Installation: `references/release-2.0.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.8.4.dll`. Profile, Spielstände und LawnGlove bleiben unverändert. Manuell insbesondere beide Rezeptrichtungen, Katzenschwanz auf Wasser und besetzte Seerosen, Dach und Unterlagen, Vorschau, Dünger, native Fähigkeiten und Projektile, Speichern/Laden sowie Almanach und Sprachwechsel prüfen.


## Änderung 1.8.4: Knopfgeometrie und Klickfläche im Fusionsalmanach

Die geklonte native Knopfvorlage enthält einen `Layout`-Container mit serialisierter Breite null. Normalerweise berechnet ein `ContentSizeFitter` die Breite anhand des lokalisierten Texts. Die Fusionsansicht deaktivierte diesen Größenanpasser, ohne dem inneren Container eine eigene Breite zu geben. Dadurch hatten Hintergrund und Klickfläche keine nutzbare Breite; der Text wurde sehr klein und untereinander dargestellt.

Fusionswechsel und Seitenknöpfe verwenden jetzt eine gemeinsame Einrichtung für feste Knopfmaße. Der innere Container und der native Hintergrund füllen die Knopffläche aus. Der Text steht mit seitlichem Abstand zentriert auf einer Zeile und verwendet 40 bis 55 Einheiten Schriftgröße. Der Hintergrund ist ausdrücklich das Raycast-Ziel; der Text überlässt Klicks dem Knopf. Die vorhandenen Klickaktionen bleiben zuständig. Native Animator-Komponenten werden ausschließlich aus den eigenen Kopien entfernt; Unity-Farbübergänge übernehmen deren Auswahl- und Klickzustände, ohne die feste Geometrie zu überschreiben. Die nativen Originalknöpfe bleiben unverändert.

1.8.4 erhöht die Fixnummer. Geometriebelege stehen unter `references/fusion-almanac/buttons-1.8.4/`, der Releasebeleg unter `references/release-1.8.4.json`. Rückbau bei geschlossenem Spiel mit `backup/FusionMod-1.8.3.dll`. Build und statische Releaseprüfung ersetzen keinen Spieltest. Spiel und Testprogramme werden nicht gestartet. Der Benutzer prüft Hintergrund, lesbare einzeilige Beschriftung, Klick auf Text und Rand, Wechsel zum Fusionsalmanach und zurück sowie Seitenknöpfe und Controller-Navigation.

## Änderung 1.8.3: Fehlender Fusionsknopf

Das vorhandene Spiellog von 1.8.2 zeigt die Ursache des fehlenden Knopfs: `Localizer.Instance` löst bereits beim Laden des generierten Interop-Typs eine `TypeLoadException` aus. Der generierte `Localizer` erbt von `Singleton<Localizer, ILocalizer>`, implementiert im verwalteten Wrapper aber `ILocalizer` nicht. Dadurch scheitert die Sprachabfrage, bevor die Almanachansicht und der Knopf „Fusions“ aufgebaut werden.

Die Sprachabfrage ruft jetzt die vorhandenen nativen Getter `Singleton.get_Instance` und `Localizer.get_LanguageLabel` über Il2CppInterop auf. Sie lädt den fehlerhaften verwalteten `Localizer`-Typ nicht. Klassen und Methoden werden über Namen aufgelöst, ohne feste Speicheradressen; fehlende Metadaten und native Ausnahmen bleiben Fehler. Nur Methodenmetadaten werden zwischengespeichert, Instanz und Sprachwert bei jeder Abfrage neu gelesen. Der Vergleich mit dem nativen Schlüssel für `Language.German`, der englische Fallback und die Aktualisierung bei Sprachwechsel bleiben erhalten. Gameplay, Pflanzenbestand und Profile ändern sich nicht.

1.8.3 erhöht die Fixnummer. Fehlerauszug und Interop-Beleg stehen unter `references/fusion-almanac/visibility-1.8.3/`. Releasebeleg: `references/release-1.8.3.json`. Rückbau bei geschlossenem Spiel mit `backup/FusionMod-1.8.2.dll`. Build und statische Releaseprüfung bestätigen keine sichtbare Laufzeitdarstellung. Das Spiel und sämtliche Testprogramme werden nicht gestartet. Der Benutzer prüft den Knopf im Pflanzenlexikon, Öffnen und Schließen der Fusionsseite sowie Deutsch, Englisch und Sprachwechsel.

## Änderung 1.8.2: Vollständig englischer und deutscher Fusionsalmanach

Alle 137 Fusionseinträge besitzen jetzt getrennte englische und vollständig übersetzte deutsche Fassungen. Namen, Fähigkeiten, Zahlenwerte, Begleittexte, Replanted-Hinweise und sämtliche 185 aktiven Rezeptalternativen erscheinen in der gewählten Fassung. Auch die bisher nur englischen deutschen Pflanzennamen sind übersetzt. Die englische Ansicht übernimmt keine deutschen Texte aus der vorherigen Mischfassung. Die eingebetteten Anzeigetexte enthalten keine chinesischen Schriftzeichen; die chinesischen Originalbelege bleiben ausschließlich im Quellenarchiv erhalten.

**Deutsch wird ausschließlich bei deutscher Spielsprache angezeigt.** Maßgeblich ist Replanteds aktive Lokalisierung: `Localizer.Instance.LanguageLabel` wird mit dem vom Spiel selbst gelieferten Schlüssel für `Language.German` verglichen. Bei Englisch und allen anderen Spielsprachen verwendet der Fusionsalmanach Englisch. Betriebssystem- und Steam-Sprache verändern diese Auswahl nicht. Ein Sprachwechsel aktualisiert auch eine bereits aufgebaute Ansicht; Auswahl, Seite und Scrollposition bleiben beim Aktualisieren der Texte erhalten. Die eigenen Seitenknöpfe, Überschriften, Kostenhinweise und Rezeptnamen wechseln ebenfalls. Der ausdrücklich gewünschte Einstiegsknopf heißt weiterhin „Fusions“.

Die deutsche Fassung liegt unter `references/fusion-almanac/translations.de.json`. Sie übersetzt die aktuellen englischen Quelltexte einschließlich der belegten Portierungsanpassungen und ersetzt die unvollständige oder zahlenmäßig veraltete deutsche Quellfassung. Für jeden Eintrag bindet `EnglishSha256` die geprüfte deutsche Übersetzung an Namen, Beschreibung, Begleittext und Hinweise der englischen Fassung. Ändern sich diese Inhalte, verlangt der Export eine erneute Übersetzungsprüfung. Der Releasecheck kontrolliert beide vollständigen Fassungen, lokalisierte Zutaten, aktuelle Rezepte, fehlende Übersetzungen und chinesische Resttexte. Neue Pflanzen benötigen laut Workflow beide Fassungen im selben Release.

1.8.2 erhöht die Fixnummer. Quellen und statische Sprachbelege stehen unter `references/fusion-almanac/localization-1.8.2/`; Release und Sicherung unter `references/release-1.8.2.json`. Rückbau bei geschlossenem Spiel über `backup/FusionMod-1.8.1.dll`. Spielstart und sämtliche Testprogramme bleiben dem Benutzer überlassen. Manuell prüfen: Englisch, Deutsch, Rückwechsel und eine andere Spielsprache als englischer Fallback; alle vier Seiten, lange Begleittexte, Zutaten und Hinweise; Schließen und erneutes Öffnen sowie Auswahl und Scrollposition beim Sprachwechsel. Build und statische Prüfung bestätigen keine Laufzeitdarstellung.

## Änderung 1.8.1: Fusionsalmanach

Im Pflanzenalmanach steht neben „Lexikon-Inhalt“ der neue Knopf **Fusions**. Die Seite enthält alle **137 implementierten Fusionsresultate**, einschließlich nativer Ergebnisse wie Repeater, Sun-shroom und Twin Sunflower. Acht Spalten mit fünf Reihen und die Knöpfe „Zurück“ und „Weiter“ verteilen die Einträge auf vier Seiten. Ein Klick auf eine Karte zeigt Originalporträt, Beschreibung, Werte, Begleittext und sämtliche tatsächlich verfügbaren Rezeptalternativen. Der gleiche Knopf wechselt als „Pflanzen“ zur normalen Pflanzenansicht zurück; die nativen Knöpfe für Lexikon-Inhalt und Schließen bleiben verfügbar.

Texte und Bilder stammen aus PvZ Fusion 4.0.5.2. Die englische und deutsche Übersetzung sind auf denselben Git-Stand wie das bestehende Rezeptarchiv festgeschrieben; die chinesischen lokalen Originaleinträge bleiben als Beleg erhalten. Deutsche Originalnamen und Begleittexte werden übernommen. Die deutsche Übersetzung ist teilweise noch englisch und enthält teilweise ältere Zahlenwerte; deshalb stammen die Verhaltensbeschreibungen aus der passenden englischen Fassung. Es handelt sich nicht um eine vollständig neue deutsche Übersetzung. Replanted verwendet seine native Schrift, Kartenrahmen, Hintergrundgrafik und Detailansicht. Die fremden festen Schriftgrößen, Ausrichtungen und Farbmarkierungen werden entfernt; die Texte erhalten Replanteds Almanachfarben und einen scrollbaren Detailbereich. Die Porträts sind statische Originalsprites, keine nachgebildeten Spielanimationen.

Belegte Abweichungen werden im Eintrag berücksichtigt: unter anderem 1800 Kontaktschaden der Giant Wall-nut, native Sun-shroom- und Twin-Sunflower-Produktion, erneute Nugget-shroom-Vermehrung nach einem erfolgreichen Zyklus und 1,5 Felder Radius der angehefteten Cherry-spreader-Bombe. Noch nicht implementierte Schnee-/Kirsch-Klickupgrades und nicht verfügbare Starry-Pumpkin-/Infernowood-Synergien werden nicht als Fähigkeiten angeboten. Kosten und Aufladezeit beschreiben die eingehende Zutatenkarte und die bestehenden Handschuhregeln. Gesperrte Alternativrezepte erscheinen nicht als ausführbare Rezepte.

Die Ansicht verwendet ausschließlich die eingebetteten Texte und Bilder. Sie registriert keine neuen nativen Pflanzentypen, erzeugt keine Pflanzen und verändert keine Freischaltungen oder Profile. Die vorhandenen nativen Pflanzenansichten werden während der Anzeige ausgeblendet und beim Zurückwechseln oder Schließen wiederhergestellt. Maus, Tastatur und Controller verwenden native Unity-Schaltflächen, explizite Navigation und den vorhandenen Scrollweg. Zur Laufzeit werden keine Quellenarchive oder Internetverbindungen benötigt.

`workflow.md` verpflichtet jetzt bei **jeder neuen Pflanze** zum passenden Almanacheintrag im selben Release. `tools/almanac.py` erzeugt die Einträge aus den aktiven Rezepten und den geprüften Zutatenidentitäten. Die bestehende Releaseprüfung kontrolliert vollständige Ergebnisabdeckung, eindeutige Quellen-IDs, aktive Rezeptalternativen, eingebettete Originalbilder und Exportprüfsummen. Quellen, Anpassungen und Bildidentitäten stehen unter `references/fusion-almanac/`.

1.8.1 erhöht die Fixnummer für den bisher fehlenden Almanach. Gameplay und Pflanzenbestand erhalten keine Änderung. Release und Sicherung werden unter `references/release-1.8.1.json` dokumentiert; Rückbau bei geschlossenem Spiel über `backup/FusionMod-1.8.0.dll`. Das Spiel wird nicht gestartet. Testprogramme und Spieltests führt der Benutzer aus. Manuell prüfen: Position des neuen Knopfs, alle vier Seiten einschließlich letzter Karte, Namen und Bilder, lange Texte und alle Rezeptalternativen, Maus-/Controller-Navigation, Scrollen sowie Wechsel zwischen Pflanzen, Fusionsseite und Lexikon-Inhalt, Schließen und erneutes Öffnen. Build und statische Quellenprüfung bestätigen keine sichtbare Spielausgabe.

## Änderung 1.8.0: Cherry-spreader und erprobter Portierungsworkflow

`Cherry Bomb | Threepeater | Cherry-spreader` ist vollständig portiert und gilt in beiden Kartenreihenfolgen sowie beiden Handschuhrichtungen. Die Pflanze besitzt 300 HP und schießt alle 1,5 Sekunden in ihre eigene Reihe und die vorhandenen Nachbarreihen. Am Spielfeldrand folgt der fehlende dritte Schuss nach 0,2 Sekunden in der eigenen Reihe. Kartenkosten und Cooldown stammen weiterhin von der eingehenden Zutat; Unterlagen, Terrain und Erzeugung vor Zutatenverbrauch verwenden den gemeinsamen Fusionsweg.

Die erste Halbkirschkugel hängt genau eine Bombe mit 20 Grundschaden an ein lebendes gegnerisches Ziel und verursacht keinen zusätzlichen direkten Treffer. Weitere Halbkirschkugeln treffen für 20 Schaden und laden die vorhandene Bombe um 5 auf. Während der Reifung erhöhen die vorhandenen Kirschgeschosse und Kirschbomben den Bombenschaden um ein Viertel ihres eingehenden Schadens, je Treffer ganzzahlig abgeschnitten. Unbeteiligte oder verschachtelt ausgelöste andere Schäden werden nicht als Kirschschaden gewertet. Die Bombe folgt ihrem Ziel und explodiert nach sechs Sekunden physisch im ursprünglichen Radius von 1,5 Quelleinheiten; im Boardmaßstab sind das etwa 88,9 Pixel. Sie erhält Kältezustände und Körper, schädigt keine Pflanzen und hinterlässt keinen Krater. Für einen fliegenden Träger gilt die getrennte Auswahl fliegender Ziele. Verschwindet oder stirbt der Träger, wird die Bombe ohne Explosion entfernt. Torchwood wandelt Halbkirschkugeln nicht um.

Die neue Pflanzenidentität `Threepeater/Cherrybomb` sowie Zielhandle, Reifezeit, Aufladung und kurze Aufladeanimation liegen in nativen gespeicherten Feldern. Es gibt keine flüchtige Bombentabelle. Pause stoppt die Reifung; Tod, Zielverlust und Pool-Wiederverwendung sind gegen alte Instanzen abgesichert. Neue Originalmodelle und Animationen für Pflanze, Flugprojektil und angehängte TimeBomb sind exportiert und eingebettet. Quellenidentität, tatsächliche Methoden, Zahlenwerte, Speicherfelder, Aufrufer und Grenzen stehen unter `references/cherry-spreader/port-notes.md` und den dort verlinkten Belegen.

Dieser Port erprobt die optimierten Werkzeuge mit einer echten neuen Fusion: gemeinsame Batch-Recherche, Export nur mit Prefab-IDs, gemeinsame Quellcaches, automatisch ergänzte Projekteinstellungen, automatisch angebotene Vorschauen und gemeinsam erzeugte Dokumentationsänderungen. Kommandos, tatsächliche Laufzeiten und Offlineprüfungen stehen unter `references/workflow-trial-cherry-spreader/`. Gemessene Einzellaufzeiten belegen keinen Vergleich mit der alten Werkzeugversion. Die erledigte Rezeptzeile ist aus `fusionen.txt` entfernt; das vollständige Quellenarchiv bleibt unverändert.

Version 1.8.0 erhöht die mittlere Stelle für eine neue Pflanze. Installation und Sicherung sind in `references/release-1.8.0.json` dokumentiert; Rückbau bei geschlossenem Spiel über `backup/FusionMod-1.7.3.dll`. Spielstände mit neuen Pflanzen oder Kirschkennungen benötigen diese oder eine kompatible neuere Version. Spiel nicht gestartet; Profile, Spielstände und LawnGlove unverändert. Der Offline-Test rekonstruiert gespeicherte Felder gegen native Doubles und ist kein tatsächlicher Save/Load- oder Unity-Spieltest. Manuell prüfen: Karten und Handschuh in beiden Richtungen, Vorschau und Markierung, Dach und Wasserunterlagen, Größen und Anker, alle drei Köpfe, Anheftung und Aufladung durch bisherige Kirschfamilien, sechs Sekunden Reifezeit, Tod und Hypnose des Trägers, Pause und reales Speichern/Laden.


## Änderung 1.7.3: Sichtbarkeit der Puff-Fusionen wiederherstellen

Der neue Größenabgleich aus 1.7.2 verwendete Meshdaten, die ebenfalls die extremen Spine-Koordinaten enthalten. Das Spielprotokoll zeigt eine Zielbox von etwa 302.795.300 × 302.795.200 Einheiten und einen Skalierungsfaktor von etwa 382 Millionen. Die Fusionen wurden dadurch weit außerhalb ihrer sichtbaren Position dargestellt. Die synthetischen Geometrietests aus 1.7.2 bildeten diesen tatsächlichen Meshinhalt nicht ab.

Die fehlerhafte Meshmessung und die dazugehörigen Tests sind entfernt. Der zuvor sichtbare Spritepfad ist wiederhergestellt. Alle importierten Puff-Fusionen werden gegenüber diesem Pfad gleichmäßig um 50 Prozent vergrößert; Seitenverhältnis, Bodenanker und horizontale Mitte bleiben erhalten. Weltgrafik und Vorschau verwenden dieselbe Vergrößerung, wobei die vorhandene Vorschau-Elternskalierung genau einmal angewendet wird. Aus der zuvor protokollierten Zielbox von 100 × 99 entstehen 150 × 148,5 Einheiten. Native Meshkoordinaten gehen nicht mehr in die Darstellung ein. Fähigkeiten, Projektile, Rezepte und Pflanzenzustände bleiben unverändert.

20 Offlineprüfungen in `tests/PuffArtworkSizingTests.csproj` prüfen die zuvor protokollierte Größenreferenz, die vergrößerten Zielwerte, das Seitenverhältnis, Welt-/Vorschauskalen und die Anker. Release-Build, Ressourcen-/Rezeptprüfung und Workflowtests ergänzen den Nachweis. Dies bestätigt keine sichtbare Spielausgabe. Spielstart und Spieltests bleiben beim Benutzer; Sichtbarkeit, Größe neben einem nativen Puff-shroom, alle Unterfelder, Vorschau, Handschuh und Speichern/Laden prüfen. Die exakte optische Übereinstimmung mit nativen Puff-shrooms ist weiterhin im Spiel zu beurteilen.

Version 1.7.3 erhöht die Fixnummer. Installation: `references/release-1.7.3.json`. Sicherung der Vorversion: `backup/FusionMod-1.7.2.dll`; diese Vorversion enthält den bekannten Sichtbarkeitsfehler. Die letzte sichtbar funktionierende ältere Version liegt unter `backup/FusionMod-1.7.1.dll`. Rückbau nur bei geschlossenem Spiel. Profile, Spielstände, LawnGlove, Assets und Workflowdateien werden nicht verändert. Nachweis: `references/puff-size-1.7.3/verification.json`.


## Änderung 1.7.2: Tatsächliche Größe der Puff-Fusionen

**Diese Änderung ist wegen des nachgewiesenen Sichtbarkeitsfehlers in 1.7.3 zurückgenommen. Die zugehörigen Geometrietests und der Mesh-Helfer sind entfernt.**

Die Größenkorrektur aus 1.6.1 änderte nur die Begrenzung des Seitenverhältnisses. Die Weltgrafik verwendete weiterhin den separat erzeugten Vorschau-Sprite als Größenreferenz. Im vorliegenden Spielprotokoll von 1.7.1 ergibt das für Pea-shroom und Repeat-shroom unverändert eine Zielbox von 100 × 99 Einheiten; der Benutzer bestätigt die weiterhin zu kleine Darstellung.

Alle importierten Puff-Fusionen messen ihre Weltgröße jetzt an den tatsächlich gezeichneten Dreiecken des nativen Puff-shroom-Meshes. Die Messung transformiert die Meshpunkte in denselben lokalen Raum wie die Importgrafik und übernimmt native Breite, horizontale Mitte und Bodenposition. Sie verwendet weder die übergroßen Spine-Cullinggrenzen noch unbenutzte Meshvertices oder einen pauschalen Vergrößerungsfaktor. Bereits wirksame Elternskalierungen, Dach- und Handschuhpositionen werden dadurch nicht doppelt angewendet. Fehlt die native Meshgeometrie noch, bleibt die native Grafik sichtbar; der Import wartet auf deren Aufbau. Die Vorschau verwendet weiterhin ihre eigene native Sprite-Größenreferenz. Fähigkeiten, Projektile, Rezepte und Pflanzenkennungen bleiben unverändert.

`tests/NativePlantArtworkTests.csproj` prüft mit produktivem Messcode und synthetischer Geometrie native Meshgröße statt Vorschaugröße, übergroße Cullinggrenzen, unbenutzte Meshkapazität, Elternskalierung, Versatz, Spiegelung und noch nicht verfügbare oder ungültige Meshdaten. Diese Offlineprüfungen messen keine laufende Spielfigur. Release-Build, Ressourcen-/Rezeptprüfung und vorhandene Workflowtests bestehen. Spielstart und visuelle Spieltests bleiben beim Benutzer; insbesondere Größe neben gewöhnlichen Puff-shrooms, alle drei Unterfelder, Dach, Handschuh, Vorschau und Laden prüfen.

1.7.2 erhöht ausschließlich die Fixnummer. Installation: `references/release-1.7.2.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.7.1.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove, Assets und beide Workflowdateien bleiben unverändert. Nachweis: `references/puff-size-1.7.2/verification.json`.


## Änderung 1.7.1: Eigene Koop-Auswahl und Platzierungsvorschau

- Die Controllerkorrektur für kleine Pflanzenfelder gilt jetzt auch für den Controller-Spieler im Koop. Jeder Controller verwendet seine eigenen Zeigerkoordinaten und seinen eigenen Rückzentrierungstimer.
- Jede Platzierungsvorschau verwendet ihren eigenen Spieler für Unterfeldposition, Gültigkeitsprüfung und Kartenauswahl. Bewegungen des Mausspielers verschieben oder blockieren die Puff-Vorschau des Controller-Spielers dadurch nicht mehr.
- Pflanzklicks behalten ihren Spielerindex. Werkzeugtreffer verwenden den Spieler und die Koordinaten des tatsächlichen Aufrufs. Die Imitater-Ausnahme prüft die Karte des zuständigen Spielers.

Das Spiel besitzt bereits getrennte Vorschauobjekte je Spieler; diese werden wiederverwendet. Der temporäre Spielerkontext verbindet native Vorschauprüfungen mit der gemeinsamen Unterfeldauswahl und wird auch nach Fehlern zurückgesetzt. Die installierte LawnGlove-Mod bleibt unverändert.

Belege und Grenzen: `references/coop-cursors-1.7.1/notes.md`. Release-Build und statische Konsistenzprüfung; bestehendes Unterfeld-Testprojekt nur kompiliert. Keine Testprogramme ausgeführt, Spiel nicht gestartet. Spieltests übernimmt der Benutzer: beide Vorschauen gleichzeitig bewegen, verschiedene Unterfelder derselben Zelle wählen, Controllerstick loslassen, platzieren und fusionieren; belegte Unterfelder, Schaufel, Kartenabbruch und Einzelspieler gegenprüfen.

Installation: `references/release-1.7.1.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.7.0.dll` nach `Mods/FusionMod.dll` kopieren.


## Änderung 1.7.0: Vier Sunflower-Fusionen

```text
Blover | Sunflower | Solar Blover
Flower Pot | Sunflower | Solar Pot
Jalapeno | Sunflower | Solar Flare
Sea-shroom | Sunflower | Sunset-shroom
```

Dies sind sämtliche vier Quellenrezepte der vier Ergebnisse. Beide Zutaten sind jeweils bereits nativ verfügbar. Alle Rezepte gelten in beiden Kartenreihenfolgen und Handschuhrichtungen. Kosten und Cooldown bestimmt die eingehende Karte. Die mittlere Versionsnummer steigt für vier neue Pflanzen. Insgesamt sind 184 Rezepte aktiv; 484 Rezepte für 414 Ergebnisse bleiben offen. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; `fusionen-quellen.json` bleibt unverändert.

- **Solar Pot:** 300 HP. Unterlage mit 15 Sonne pro Produktionszyklus; Grundintervall 25 Sekunden, originale Abweichung ±10 Prozent. Nach Beginn der Aufhellung erfolgt die Auszahlung eine halbe Sekunde später. Ein normaler Bewohner bleibt beim Fusionieren des darunterliegenden Topfs erhalten. Wenn auch die normale Pflanze ein passendes Rezept hat, erhält dieses Vorrang: Eine Karte erzeugt nur ein Ergebnis. Vorhandene andere Unterlagen werden nicht ersetzt. Die native Topffunktion und deren Terrainregeln bleiben zuständig.
- **Sunset-shroom:** 300 HP. Wasserpflanze ohne Schüsse. Produziert zunächst 15, nach 20 aktiven Sekunden 25 und nach weiteren 40 Sekunden 50 Sonne. Produktionsintervall 15 Sekunden mit ±5 Prozent Abweichung und einer halben Sekunde Aufhellung bis zur Auszahlung. Beide originalen Wachstumsclips und alle drei Altersdarstellungen sind eingebunden. Schlaf, Kaffee und Wasserplatzierung bleiben nativ. Dünger heilt und verkürzt nach der Fütterungsanimation die laufende Wachstumsstufe auf höchstens 0,5 aktive Sekunden; er überspringt keine Stufe. Eine zweite Anwendung beschleunigt die zweite Stufe. Dieses Verhalten folgt `Fertilize.Upgrade` und `SunSeaShroom.AttributeEvent` der lokalen Quelle.
- **Solar Flare:** 300 HP. Explodiert zum Originalereignis bei 2/3 Sekunde, auch bei vorheriger Zerstörung. Verursacht in ihrer Reihe 1800 Ascheschaden plus 500 für jede erfasste eigene Pflanze. Jede getroffene gegnerische Zombieinstanz erzeugt 25 Sonne. Anschließend erhalten eigene Pflanzen derselben Reihe jeweils 1800 Schaden; pro Pflanze entstehen 50 Sonne. Die Quelle lässt mit Wahrscheinlichkeit 1/3 genau 1 HP übrig, sofern die Gesundheit durch den Treffer strikt negativ wird. Bei exakt 0 HP greift diese Ausnahme nicht. Andere Reihen und hypnotisierte Zombies bleiben verschont. Zutatenverbrauch und Aufräumen eines Boards lösen keine Explosion aus. **Die Fähigkeit zerstört bewusst eigene Pflanzen, einschließlich betroffener Unterlagen und Kürbisse.**
- **Solar Blover:** 300 HP. Originales Blasereignis nach rund 0,49 Sekunden. Entfernt feindliche Ballonzombies und vertreibt Nebel für 30 Sekunden. Feuert anschließend 72 kleine Sonnenprojektile in zwei Spiralen mit Winkeln 10 bis 720 Grad, je eines pro 0,02 Sekunden. Jedes Projektil verursacht 10 Schaden und hinterlässt bei einem Zombietreffer einen sammelbaren Pickup mit 5 Sonne. Keine Torchwood-Umwandlung. Nach dem letzten Coroutine-Intervall verschwindet die Pflanze.

Originalgrafik und Animationen stammen ausschließlich aus dem lokalen 4.0.5.2-Bundle. Native Partikel und Sounds ersetzen Quellenpartikel. Alle fünf Modelle, 13 Clips und 27 Sprites sind exportiert und eingebettet; die Offlinevorschauen sind visuell kontrolliert. Quellbeschreibungen, Datenwerte, Rezepte, Scriptidentitäten und Prüfsummen liegen unter `references/solar-pot/`, `sunset-shroom/`, `solar-flare/`, `solar-blover/` und `blover-sun-projectile/`. Verhaltenswege, native Speicherung, Terrainabfragen und Grenzen erläutert `references/solar-fusions-native/port-notes.md`.

Neue Speicherpaare sind Flowerpot/Sunflower, Seashroom/Sunflower, Jalapeno/Sunflower und Blover/Sunflower. Produktions-, Wachstums- und Schussphasen sowie die Projektilkennung liegen in nativen gespeicherten Feldern. Laden rekonstruiert Identität, Phase und Darstellung. Die vorhandene Sonnenhilfe speichert den Sonderwert bereits ausgezahlter 5-Sonnen-Pickups nur flüchtig: Ein vor dem Einsammeln geladener solcher Pickup fällt auf den nativen SmallSun-Wert zurück. Die regulären 15/25/50-Sonnen-Pickups bleiben nativ gespeichert. Spielstände mit den neuen Varianten benötigen diese oder eine kompatible neuere Version.

Offline geprüft: produktive Fähigkeiten, Düngerabschluss und DataID-Schutz, Produktionswerte, beide Wachstumsstufen, Pausen und Schlaf, 72 Schüsse, Treffer, Eigenbeschuss, Tod, gespeicherte Zwischenphasen, Karten-/Handschuhwege sowie Unterlagenerhalt und atomare Erzeugung. Rezept-/Assetkonsistenz, Workflowprüfungen, betroffene Regressionen und Release-Build sind separat dokumentiert. Spiel nicht gestartet; Profile und Spielstände nicht geändert. Beide Workflowdateien bleiben unverändert. Installation: `references/release-1.7.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.6.2.dll` nach `Mods/FusionMod.dll` kopieren.

Manuell prüfen: alle vier Rezepte in beiden Reihenfolgen mit Karten und Handschuh, Unterlagen, belegte Töpfe, Vorschau, Markierung, Kosten und Cooldown; Wasser, Dach, Tag/Nacht und Kaffee; Sonnenwerte, Wachstum und zweimaliges Düngen; Solar Flares Eigenbeschuss und Bonus; Ballons, Nebel und alle 72 Solar-Blover-Projektile. Pause, Tragen, Bungee, Tod und Speichern/Laden während Produktion, Wachstum, Fütterung, Spirale und Projektilflug prüfen.

## Änderung 1.6.2: Controllercursor und Unterfeldauswahl

- Dünger entfernt nach erfolgreicher Verwendung auch mit Controller den Werkzeugcursor. Aufnahme, Abbruch, Speichern und Aufräumen verwenden ebenfalls die native Cursorlöschung unabhängig vom Steuerungstyp. Bei ungültigen Zielen bleibt der Beutel verfügbar.
- Auf kleinen Pflanzenfeldern entfällt die automatische Rückzentrierung des Controllerzeigers. Die per Stick gewählte Position bleibt dadurch für Handschuh, Schaufel und Platzierung erhalten. Außerhalb dieser Felder bleibt die native Rückzentrierung aktiv; Steuerkreuznavigation bleibt unverändert.
- Die Schaufel löst überlappende kleine Pflanzen anhand des gewählten Unterfelds auf. Leere Unterfelder wählen keine benachbarte kleine Pflanze aus. Native Priorität von Kürbis und Unterlagen bleibt erhalten.

Native Belege und technische Grenzen: `references/controller-fixes-1.6.2/notes.md`. Release-Build und statische Konsistenzprüfung; keine Testprogramme ausgeführt. Spiel nicht gestartet. Spieltests übernimmt der Benutzer: alle drei Unterfelder mit Stick wählen, loslassen, Handschuh/Schaufel verwenden, freie Unterfelder bepflanzen sowie Dünger verwenden und abbrechen.

Installation: `references/release-1.6.2.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.6.1.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.


## Änderung 1.6.1: Projektile, Größen, Sun-shroom und Dünger

- Peashooter Mine (Peatato Mine) behält jetzt die originale Kartoffelgeschossgrafik. Die Gültigkeitsprüfung des Importrenderers ordnet diese Geschosse ihrer eigenen Projektilfamilie zu; zuvor fiel sie auf den Cherry-Shooter-Test zurück und entfernte die Grafik sofort. Normale Schüsse und alle 36 radialen Todesschüsse verwenden dasselbe Originalasset aus 4.0.5.2. Schaden, Geschwindigkeit und Kollision bleiben beim vorhandenen Kartoffelprojektilpfad.
- Giant Wall-Nut verwendet Größe 1,5 statt 1,8 und eine passende 120-Pixel-Kontaktbox. Die Bodenanimation läuft mit 5,4 statt 12 Bildern pro Sekunde, auch bei einer schon laufenden oder geladenen Animation. Die Bewegung normalisiert Größe und Animationsrate getrennt; Fließkomma-Bewegung vermeidet Stillstand und Sprünge durch Abschneiden kleiner Schritte.
- Puff-shroom + Sunflower erzeugt ausschließlich den nativen Replanted-Sun-shroom. Die zusätzliche Portierung, Sonnenproduktion und eingebetteten Sun-shroom-Assets sind entfernt. Native Fähigkeiten, Kaffee, Wachstum, Dünger und Darstellung sind zuständig. Das alte Speicherpaar Puffshroom/Sunflower bleibt für die Umstellung reserviert und wird nicht neu vergeben. Vor der Controllererzeugung werden alte portierte Pflanzen in native Sun-shrooms umgestellt; Gesundheit, Position, Reifezeit und Produktionsabstand bleiben erhalten. Die alte portierte Aufhellungsphase wird beendet. Die Angaben zur separaten Portierung im historischen Abschnitt 1.5.0 gelten damit nicht mehr.
- Ein Düngerbeutel heilt alle lebenden Pflanzen derselben Zelle: sämtliche drei Pilzunterfelder, Kürbis, Unterlage und andere Schichten. Die tatsächliche Pflanzenliste ersetzt die auf einen normalen Bewohner begrenzte Schichtabfrage. Jede wachstumsfähige Pflanze erhält einen eigenen gespeicherten Auftrag mit DataID im serialisierten Werkzeugzähler. Alle jungen nativen Sun-shrooms wachsen nach Ende der Dünganimation; entfernte, bewegte oder getragene Pflanzen erhalten kein fremdes Wachstum. Eine inzwischen eingesetzte Ersatzpflanze übernimmt keinen alten Auftrag. Vor Heilung werden sämtliche Aufträge reserviert; bei Allokationsfehlern bleiben Pflanzen unverändert. Vor 1.6.1 gespeicherte Düngeraufträge verwenden weiterhin ihre bisherige Auflösung.
- Puff-Fusionen werden anhand der nativen Puff-shroom-Breite skaliert. Die bisherige zusätzliche Höhenbegrenzung verkleinerte hohe Fusionen unter die native Breite. Seitenverhältnis, Bodenanker und vorhandene Vorschau-Skalierung bleiben erhalten.

Es werden keine neuen Pflanzen oder Rezepte hinzugefügt. 1.6.1 erhöht ausschließlich die letzte Versionsnummer für Fehlerkorrekturen. Originalquellen und Assets stammen weiterhin aus dem lokalen `english-4.0.5.2`; archivierte Sun-shroom-Quellen bleiben als Nachweis erhalten. `fusionen.txt`, `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert. Der bestehende Workflow deckt diese Korrekturen bereits ab.

Offline-Nachweise: `tests/TileFixesTests.csproj` prüft vollständige Felddüngung, drei gespeicherte Wachstumsaufträge, Animation, Ersatzziele, Bewegung, Handschuh, Allokationsfehler, Kürbiserhalt, Minenbewaffnung, Altstand-Umstellung und Walnussbewegung. Bestehende Puff-, Unterfeld- und Minenregressionen sowie Ressourcen-/Rezeptprüfung, Workflowprüfungen und Release-Build ergänzen den Nachweis. Native Belege und Grenzen stehen unter `references/fixes-1.6.1/`. Kein Spielstart und keine Spieltests; Profile und Spielstände werden nicht bearbeitet.

Manuell prüfen: Kartoffelgeschosse vorwärts und nach Minentod, Größe der Puff-Fusionen neben gewöhnlichen Puff-shrooms einschließlich Vorschau und Handschuh, Größe und Rotation der Giant Wall-Nut, Sun-shroom-Rezept in beiden Richtungen und vorhandene ältere Sun-shrooms nach Laden. Dünger auf drei junge Sun-shrooms, gemischte Pilze, Kürbis mit Pflanze und Minen anwenden; Heilung, Wachstum erst nach Animation, Pause und Speichern/Laden während des Düngens prüfen. Die tatsächliche visuelle Übereinstimmung und die Controllerwiederherstellung alter Sun-shrooms benötigen diese Spielprüfung.

Installation: `references/release-1.6.1.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.6.0.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert. Ein nach Umstellung gespeicherter Sun-shroom ist eine gewöhnliche native Pflanze.


## Änderung 1.6.0: Buck-shroom, Sprout-pult, Baby Corn und Slice-pult

```text
Pea-shroom | Bucket | Buck-shroom
Puff-shroom | Buckshooter | Buck-shroom
Puff-shroom | Cabbage-pult | Sprout-pult
Puff-shroom | Kernel-pult | Baby Corn
Puff-shroom | Melon-pult | Slice-pult
```

Alle Zutaten waren bereits vollständig verfügbar. Diese fünf Rezepte sind sämtliche Quellenrezepte der vier Ergebnisse. Die gemeinsame Rezeptauflösung unterstützt beide Zutatenreihenfolgen, Karten und Handschuh, ein bis drei passende Puff-Unterfelder, Terrain, Unterlagen, Markierung und Vorschau. Die Ergebnisse entstehen vor dem Zutatenverbrauch; Kosten und Cooldown bleiben bei der eingehenden Zutat. Die fünf erledigten Zeilen sind aus `fusionen.txt` entfernt. Insgesamt sind 180 Rezepte aktiv; 488 Rezepte für 418 Ergebnisse bleiben offen. Folgefusionen und `fusionen-quellen.json` bleiben unverändert.

- **Buck-shroom:** 1000 HP. Schießt alle 2,25 Sekunden kleine originale Eisenprojektile über die ganze Reihe. Jeder Treffer verursacht 80 Schaden zusätzlich zur aktuellen Schildgesundheit und stößt geeignete Gegner über den vorhandenen Eisenadapter zurück. Schwere Gegner behalten dessen Rückstoßimmunität. Die Pflanze zählt beim vorhandenen Eimerpfad als Eisenpflanze: Ein weiterer Bucket heilt bis zu 500 HP, höchstens bis zur bisherigen Maximalgesundheit. Beide Quellenrezepte sind umgesetzt. Die Eisenprojektile werden nicht durch Torchwood entzündet.
- **Sprout-pult:** 300 HP. Wirft alle drei Sekunden kleine Kohlköpfe mit 40 direktem Schaden. Die Geschosse können zweimal abprallen, insgesamt drei verschiedene aufeinanderfolgende Ziele treffen. Horizontalgeschwindigkeit nach einem Treffer: 40 Prozent; reflektierte Vertikalgeschwindigkeit: 50 Prozent. Bodenkontakt beendet den Flug. Trifft ein Kohlkopf einen geschrumpften Zombie, erhält dieses Geschoss die Schrumpfwirkung. Spätere Treffer schrumpfen geeignete Ziele vor dem Schaden einmalig auf 70 Prozent Größe und aktueller/maximaler Körper-, Helm- und Schildgesundheit. Ohne vorherigen Kontakt mit einem geschrumpften Gegner besitzt der Kohlkopf keine Schrumpfwirkung.
- **Baby Corn:** 300 HP. Wirft alle drei Sekunden kleine Maiskörner mit 20 Schaden, mit 25 Prozent Wahrscheinlichkeit stattdessen kleine Butter mit 40 Schaden. Sucht zuerst den nächsten noch nicht geschrumpften Bodenzombie vor sich in derselben Reihe; falls keiner vorhanden ist, den nächsten geschrumpften Gegner. Kleine Butter schrumpft ein geeignetes ungeschrumpftes Ziel einmalig. Bei bereits geschrumpften Zielen immobilisiert sie stattdessen für 1,5 Sekunden; längere vorhandene Butter bleibt erhalten. Kleine Butter tötet Imps. Native Butterimmunität und die vorhandene Bossimmunität gegen Schrumpfen bleiben erhalten.
- **Slice-pult:** 300 HP. Wirft alle drei Sekunden originale Melonenstücke mit 80 direktem Schaden, gegen geschrumpfte Gegner 160. Diese Stücke verursachen keinen Flächenschaden.

Alle Angriffsintervalle verwenden fünf Prozent Zufallsabweichung. Die Wurfpflanzen suchen Bodenziele vorwärts in derselben Reihe über die volle Reihe. Das Originalereignis der drei Wurfclips liegt bei 0,616667 Sekunden, beim Buck-shroom bei 0,383333 Sekunden. Baby Corn speichert die Butterentscheidung schon beim Beginn des zugehörigen Originalclips. Native Wurfbewegung und Kollision bilden den Quellenflug von 1,5 Sekunden ab. Geworfener Direktschaden umgeht Handschilde; Körper-, Helm-, Todes- und Beuteregeln bleiben beim nativen Gesundheitsschreiber. Schlaf/Kaffee, Pause, Handschuh, Bungee und Tod verwenden die vorhandenen Lebenszyklusregeln. Originalanimationen erzeugen keine zweiten Schüsse.

Originalprefabs: IronPuffPrefab 57323, CabbagePuffPrefab 33774, CornPuffPrefab 33773 und MelonPuffPrefab 33775. Alle neun Pflanzenclips und fünf tatsächlichen Originalprojektilprefabs sind exportiert. Buck-shroom verwendet ausdrücklich Bullet_puffIronPea 34018 mit dem Script Bullet_ironPea, nicht den ähnlich benannten Flugabwehrtyp Bullet_ironPea_air. Mais, Butter, Kohl und Melone verwenden 75723, 42997, 48867 und 72616. Originalsprites, Pivots, Hierarchie, Schatten und Controller bleiben erhalten; der vorhandene Importrenderer verwendet native Puff-Größenreferenzen. Projektile bleiben am nativen Bewegungsanker, ihre Schatten am Boden. Native Trefferpartikel ersetzen Quellenpartikel. Es werden keine fremden Unity-Spielskripte geladen und keine unbekannten Animationsbindungen ausgelassen.

Neue Speicherpaare: Puffshroom/Magnetshroom, Puffshroom/Cabbagepult, Puffshroom/Kernelpult und Puffshroom/Melonpult. Die Wurfpflanzen speichern Abstand, Clipphase und Butterwahl in `mLaunchCounter`, `mStartRow` und `mShootingCounter`. Die Geschosse speichern Familie und bei Kohl auch Trefferzahl und Schrumpfwirkung in `mCobTargetX`; `mTargetZombieID` verhindert den sofortigen erneuten Treffer desselben Ziels. Native Geschwindigkeits- und Beschleunigungsfelder erhalten die Flugbahn beim Laden. Der bestehende gespeicherte Größenwert identifiziert geschrumpfte Zombies. Es gibt keinen neuen flüchtigen Fähigkeitszustand. Spielstände mit diesen Varianten benötigen diese oder eine kompatible neuere Version.

Quellenbeschreibungen, alle Rezepte, Methoden, Datenwerte, Scriptidentitäten, Asset-Prüfsummen und Vorschauen: `references/buck-shroom/`, `sprout-pult/`, `baby-corn/`, `slice-pult/`, `buck-shroom-projectile/`, `puff-lob-source/`, `puff-lob-base/` und `puff-lob-native/`. Der gemeinsame Verhaltens- und Speicherbeleg liegt in `references/puff-lob-native/port-notes.md`.

Offline geprüft: produktive Fähigkeiten, vollständige Rezepte in beiden Reihenfolgen, originale Ereignisphasen, geschützte Timer, gespeicherte Butterwahl, native Flugparameter, Kohl-Abpraller und Schrumpfweitergabe, 20/40/80/160 Direktschaden, fehlender Melonen-Flächenschaden, Imp-Sonderfall, Eimerheilung, Schildbonus und Torchwood-Ausnahme. Vorhandene Puff-, Unterfeld-, Eimer-, Football-, Scaredy- und Squash-Regressionen sowie Rezept-/Assetkonsistenz, Workflowprüfungen und Release-Build sind geprüft. Originalanimationsvorschauen sind offline kontrolliert. Diese Nachweise bestätigen keine Spieltests. Das Spiel wurde nicht gestartet; Profile und Spielstände bleiben unverändert. Version 1.6.0 erhöht die mittlere Nummer für neue Pflanzen. Installation: `references/release-1.6.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.5.0.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove und beide Workflowdateien bleiben unverändert.

Manuell durch den Benutzer prüfen: alle fünf Rezepte mit beiden Zutatenreihenfolgen, Karten und Handschuh, ein bis drei sowie gemischte Unterfelder, Unterlagen, Terrain einschließlich Dach/Pool, Kosten, Cooldown, Vorschau, Markierung und Abbruch. Buck-shrooms 1000 HP, Schildbonus, Rückstoß und Eimerheilung prüfen. Drei Wurfarten, kleine Butter samt Imp-Sonderfall, Kohl-Abpraller und Schrumpfweitergabe sowie Melonenstücke ohne Flächenschaden prüfen. Schlaf/Kaffee, Pause, Geschwindigkeit, Handschuh, Bungee, Tod und Speichern/Laden vor und nach dem Schussereignis, während Butterclip, Flug, Abprall und nach Schrumpfen gegenprüfen.


## Änderung 1.5.0: Sun-shroom, Gnome-shroom und Umbrella Cap

```text
Puff-shroom | Sunflower | Sun-shroom
Puff-shroom | Hypno-shroom | Gnome-shroom
Puff-shroom | Umbrella Leaf | Umbrella Cap
```

Alle drei Ergebnisse besitzen jeweils genau dieses eine Quellenrezept. Beide Zutaten sind vollständig verfügbar. Die gemeinsame Rezeptauflösung unterstützt beide Kartenreihenfolgen und Handschuhrichtungen, ein bis drei Puff-Unterfelder, atomare Erzeugung vor Zutatenverbrauch, Terrain, Unterlagen, Vorschau und Hervorhebung. Kosten und Cooldown stammen von der eingehenden Zutat. Die drei erledigten Zeilen sind aus `fusionen.txt` entfernt. Insgesamt sind 175 Rezepte aktiv; 493 Rezepte für 422 Ergebnisse bleiben offen. Folgefusionen und `fusionen-quellen.json` bleiben unverändert.

- **Sun-shroom:** 300 HP. Produziert alle 25 Sekunden mit fünf Prozent Zufallsabweichung zunächst 15 Sonne, nach 120 aktiven Sekunden 25 Sonne. Die originale Aufhellung beginnt mit dem Produktionszyklus; die Auszahlung erfolgt eine halbe Sekunde später. Originale Wachstumsanimation und beide Altersdarstellungen sind eingebunden. Native SmallSun-/Sun-Pickups erhalten Wert, Sammlung und Speicherung.
- **Gnome-shroom:** 300 HP, keine Schüsse. Ein Biss in den wachen Pilz hypnotisiert den angreifenden Zombie und verbraucht den Pilz. Der Zombie schrumpft einmalig auf 70 Prozent seiner bisherigen Größe. Aktuelle und maximale Körper-, Helm- und Schildgesundheit werden jeweils mit 0,7 multipliziert und abgerundet. Bereits geschrumpfte Zombies schrumpfen nicht erneut. Native Sonderfälle ohne Pflanzenbiss beziehungsweise Hypnose bleiben ausgeschlossen.
- **Umbrella Cap:** 300 HP. Vermehrt sich alle 30 aktiven Sekunden im ersten freien Unterfeld derselben Zelle. Auch nach einem vollen Feld wird der Elterntimer erneut gestartet. Naher Bodenkontakt in derselben Reihe startet den originalen halben Sekunden langen Blockclip. Zum Originalereignis bei einer Sechstelsekunde werden geeignete Gegner derselben Reihe im Radius einer Quelleinheit ohne Schaden zwei Quelleinheiten zurückgestoßen. Ein ungeschrumpfter Gegner verbraucht den Pilz. Sind nur geschrumpfte Gegner vorhanden, verliert der Pilz stattdessen 50 HP. Die tatsächliche Methode `SmallUmbrella.AnimBlock` verlangt diese 50 HP auch dann, wenn bis zum Ereignis alle Gegner den Radius verlassen haben. Diese Kleinvariante schützt keine Nachbarpflanzen vor Katapulten oder Bungees.

Alle Fähigkeiten verwenden native Simulationsticks; Schlaf, Pause und Tragen stoppen sie. Die drei Originaltypen erzeugen keine Projektile. Native Puff-Sporen werden für diese Varianten unterdrückt. Die Originalprefabs SunShroom 50766 / Transform 116220 / Controller 6495, HypnoPuff 56884 / 95285 / 6605 und SmallUmbrella 53646 / 90910 / 6554 stammen ausschließlich aus dem lokalen 4.0.5.2-Bundle. Alle 15 Sprites und sechs Clips sind eingebunden, ohne ausgelassene Bindungen. Schatten, Hierarchie, Pivots, Puff-Größenreferenz und Markierung verwenden den bestehenden Renderer. Sonnenpickups, Hypnose und Rückstoß verwenden native Replanted-Adapter; Unity-Spielskripte werden nicht geladen.

Speicherpaare: Puffshroom/Sunflower, Puffshroom/Hypnoshroom und Puffshroom/Umbrella. Produktion, Aufhellung, Alter, Wachstumsclip, Vermehrung und Blockphase liegen in den vorhandenen gespeicherten Pflanzenfeldern. Schrumpfen nutzt das gespeicherte native `Zombie.mScaleZombie` sowie die sechs Gesundheitsfelder. Die drei nativen Ausgangsgrößen 1, 0,8 und 0,5 ergeben eindeutig erkennbare Schrumpfgrößen 0,7, 0,56 und 0,35. Laden rekonstruiert Darstellung und Zustand ohne erneutes Schrumpfen oder zusätzliche Sonne. Fremdmods mit anderen Zombiegrößen sind nicht geprüft. Spielstände mit diesen Varianten benötigen diese oder eine kompatible neuere Version.

Originalbeschreibungen, vollständige Rezepte, tatsächliche Methoden, Rohdaten, Asset-Prüfsummen und Vorschauen liegen unter `references/sun-shroom/`, `gnome-shroom/` und `umbrella-cap/`. Aufruf-, Speicher- und Portierungsbelege: `references/puff-support-1-5/port-notes.md`.

Offline bestanden: 155 Fähigkeitsprüfungen und 423 Unterfeldprüfungen, einschließlich beider Rezeptreihenfolgen, Handschuhrichtungen, ein bis drei Unterfeldern, Wachstum, Produktionszeitpunkt, gespeicherter Zwischenphasen, Schrumpfen aller sechs Gesundheitsfelder, Rückstoß, gemischter Gegnergrößen und Vermehrung bei vollem Feld. Zusätzlich bestanden: 91 Puff-Spezialprüfungen, 195 Puff-Schützenprüfungen, 139 Scaredy-Prüfungen und 13 Workflowprüfungen. Originalanimationsvorschauen sind visuell kontrolliert; Rezept-/Assetkonsistenz und Release-Build ohne Warnungen oder Fehler sind geprüft. Diese Prüfungen bestätigen keine Spieltests. Das Spiel wird nicht gestartet; Profile und Spielstände bleiben unverändert. Version 1.5.0 erhöht die mittlere Nummer für neue Pflanzen. Installation: `references/release-1.5.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.4.0.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove und beide Workflowdateien bleiben unverändert.

Manuell durch den Benutzer prüfen: alle drei Rezepte mit Karten und Handschuh in beiden Richtungen, teilweise belegte und gemischte Unterfelder, Terrain, Unterlagen, Kosten, Cooldown, Vorschau und Markierung. Sun-shrooms 15/25 Sonne, Wachstum und Auszahlung prüfen; Gnomes Bisshypnose, Körpergröße und Rüstung prüfen; Umbrella Caps Vermehrung, Rückstoß, fehlenden Schaden, Verbrauch bei großen Gegnern und 50 HP bei kleinen Gegnern prüfen. Schlaf/Kaffee, Pause, Geschwindigkeit, Tod und Speichern/Laden während Wachstum, Aufhellung, Blockphase und nach Schrumpfen gegenprüfen.


## Änderung 1.4.0: Hail-shroom, Icicle-shroom, Twinkle-shroom und Nugget-shroom

```text
Hail-shroom | Peashooter | Icicle-shroom
Pea-shroom | Ice-shroom | Icicle-shroom
Puff-shroom | Ice-shroom | Hail-shroom
Puff-shroom | Potato Mine | Nugget-shroom
Puff-shroom | Snow Pea | Icicle-shroom
Puff-shroom | Starfruit | Twinkle-shroom
```

Alle vier Ergebnisse sind mit bereits verfügbaren Zutaten portiert. Hail-shroom wird zuerst verfügbar; damit sind auch sämtliche drei Quellenrezepte für Icicle-shroom zulässig. Alle sechs Rezepte verwenden die gemeinsame Auflösung für beide Kartenreihenfolgen und Handschuhrichtungen. Passende Puff-Unterfelder fusionieren gemeinsam; Unterfelder, unpassende Pflanzen, Seerosen, Töpfe und Kürbisse bleiben erhalten. Kosten und Cooldown bestimmt die eingehende Zutat. Ergebnisse entstehen vor dem Zutatenverbrauch. Die sechs erledigten Zeilen sind aus `fusionen.txt` entfernt. Insgesamt sind 172 Rezepte aktiv; 496 Rezepte für 425 Ergebnisse bleiben offen. Folgefusionen und `fusionen-quellen.json` bleiben unverändert.

- **Hail-shroom:** 300 HP. Ein naher, feindlicher Bodenzombie derselben Reihe löst vier Sekunden Frost mit anschließend zehn Sekunden Kälte aus. Bereits gefrorene Kontakte lösen nicht erneut aus. Die Pflanze verschwindet nach der Wirkung. Kein Schaden und kein Projektil; native Frostimmunität bleibt zuständig.
- **Icicle-shroom:** 300 HP. Sucht vorwärts in derselben Reihe bis 2,5 Quelleinheiten, etwa 1,85 Felder. Schießt im Grundintervall von 2,25 Sekunden originale kleine Eisgeschosse mit 20 Schaden, Kälte und zehn Frostpunkten. Ein gefrorenes oder kälteimmunes Ziel erhält vierfachen Schaden. Die tatsächliche lokale Methode `Bullet_snowPea.HitZombie` addiert zehn Frostpunkte und entfernt das Geschoss nach dem ersten Treffer. Die Almanachangaben „fünf Frostpunkte“ und „zweimal durchdringen“ entsprechen dieser ausgelieferten Implementierung nicht; der Port folgt dem tatsächlichen Code. Die unabhängige Zufallsaufwertung durch eine zusätzliche Quellenfähigkeit gehört nicht zur Grundpflanze.
- **Twinkle-shroom:** 300 HP. Native Sternzielsuche aktiviert alle 2,25 Sekunden fünf originale Sterne mit je 20 Schaden. Richtungen: 30, -30, 180, 90 und -90 Grad. Die beiden schrägen und beiden senkrechten Sterne fliegen mit fünf Quelleinheiten pro Sekunde, der rückwärtige Stern mit sechs. Originales Schussereignis bei 0,25 Sekunden; Reihenwechsel, Rüstung und Kollision bleiben nativ.
- **Nugget-shroom:** 300 HP. Schießt originale Kartoffelsporen mit 20 Schaden im tatsächlichen Grundintervall von zwei Sekunden. Die Zielreichweite beträgt ebenfalls 2,5 Quelleinheiten. Nach etwa 20 aktiven Sekunden entsteht im ersten freien Unterfeld derselben Zelle ein weiterer Nugget-shroom mit eigenem Timer. Die tatsächliche Methode `PotatoPuff.AttributeEvent` startet nach erfolgreicher Vermehrung auch den Elterntimer erneut. Misslingt die Platzierung, etwa bei drei belegten Unterfeldern, stoppt dieser Timer dauerhaft. Das unterscheidet sich von der Almanachformulierung „einmal pro Pflanze“. Gewöhnlicher Tod verursacht im Umkreis einer Quelleinheit 300 Kartoffel-Explosionsschaden gegen geeignete Bodenziele derselben Reihe. Zutatenverbrauch, fehlgeschlagene Fusion und Board-Abbau lösen keine Explosion aus. Kein Krater; Kartoffelsporen verursachen keine zusätzliche Explosion.

Alle Schussintervalle und der Vermehrungstimer behalten die ursprüngliche Zufallsabweichung von fünf Prozent. Icicle- und Nugget-Geschosse fliegen mit sechs Quelleinheiten pro Sekunde; 40 Boardpixel Flughöhe halten sie oberhalb der nativen Bodentreffergrenze. Normales Torchwood wandelt beide Geschossarten nicht um. Schlaf, Pause, Handschuh und Bungee stoppen die Pflanzenfähigkeiten. Es entstehen keine zusätzlichen Fusionsboni oder zweiten Schüsse durch Animationsevents.

Originalsprites und sämtliche sieben Pflanzenclips stammen aus dem lokalen 4.0.5.2-Bundle: SmallIceShroomPrefab 56882 / Transform 95283 / Controller 6631; IcePuffPrefab 42855 / 98233 / 6457; StarPuffPrefab 42857 / 98235 / 6460; PotatoPuffPrefab 42859 / 98237 / 6454. Das neue Originalgeschoss Bullet_snowPuff 73650 / 95613 besitzt tatsächlich das Script `Bullet_snowPea`. Nugget verwendet den vorhandenen identischen Export Bullet_puffPotato 73652; Twinkle verwendet Bullet_star 33973. Hierarchie, Schatten, Pivots, Animationen und die native Puff-Größenreferenz bleiben erhalten. Alle Bindungen sind exportiert; keine neue Ausnahme wird eingeführt. Native Frost-, Treffer- und Explosionspartikel sowie Audio ersetzen entsprechende Quellenkomponenten. Unity-Spielskripte werden nicht geladen.

Speicherpaare: Puffshroom/Iceshroom, Puffshroom/Snowpea, Puffshroom/Starfruit und Puffshroom/Potatomine. `mLaunchCounter` speichert den Schussabstand, `mStartRow` die Clipphase und `mTargetX` den Nugget-Vermehrungstimer einschließlich des gestoppten Zustands null. Nachkommen werden mit ihrer Ergebniskennung erzeugt, unabhängig von Cursor und Spieler-Fusionskontext. Kleine Eisgeschosse beziehungsweise Kartoffelsporen speichern -470018/-470092 in `Projectile.mCobTargetX`; Bewegung, Schaden und Darstellung bleiben nach dem Laden erhalten. Die gemeinsame vorhandene SnowPea-Verwaltung für noch nicht ausgelöste Frostpunkte und steigende Frostschwellen ist weiterhin flüchtig und wird beim Laden zurückgesetzt. Aktiver Frost und Kälte speichern nativ. Spielstände mit diesen Varianten benötigen diese oder eine kompatible neuere Version.

Quellen, unveränderte Originalbeschreibungen, vollständige Rezepte, tatsächliche Methoden, Originaldaten, Asset-Prüfsummen und Vorschauen liegen in `references/hail-shroom/`, `icicle-shroom/`, `twinkle-shroom/` und `nugget-shroom/`. Ausführlicher Verhaltens- und Aufrufbeleg: `references/puff-special-native/port-notes.md`.

Offline geprüft: produktive Fähigkeiten, sechs Rezepte in beiden Reihenfolgen, Karten- und Handschuhfusionen, ein bis drei Unterfelder, Vermehrung ohne Cursorabhängigkeit, volle Felder, fehlgeschlagene Erzeugung, gespeicherte Timer, Tod, Rückbau und bestehende Puff-/Scaredy-/Stern-/Kälteregressionen. Originalanimationsvorschauen, Rezept-/Assetkonsistenz, Workflowprüfungen und Release-Build sind geprüft. Dies bestätigt keine Spieltests. Das Spiel wurde nicht gestartet; Profile und Spielstände bleiben unverändert. Version 1.4.0 erhöht die mittlere Nummer für neue Pflanzen. Installation: `references/release-1.4.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.3.0.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove und beide Workflowdateien bleiben unverändert.

Manuell durch den Benutzer prüfen: sämtliche sechs Rezepte mit Karte und Handschuh in beiden Richtungen; gemischte und teilweise belegte Unterfelder, Kosten, Cooldown, Terrain, Unterlagen, Abbruch, Vorschau und Hervorhebung. Hail-Kontaktfrost, Kälte und Immunität; Icicle-Reichweite, 20/80 Schaden, zehn Frostpunkte und Ende am ersten Treffer; Twinkles fünf Richtungen und entfernte Ziele; Nuggets 20-Sekunden-Vermehrung, freie Unterfelder, reaktivierten Elterntimer, Stopp bei vollem Feld und 300 Todesschaden. Schlaf/Kaffee, Pause, Geschwindigkeit, Handschuh, Bungee, Tod und Speichern/Laden während Schussclips, Vermehrung und Geschossflug gegenprüfen.


## Änderung 1.3.0: Vier weitere Puff-shroom-Fusionen

```text
Puff-shroom | Wall-nut | Nut-shroom
Puff-shroom | Jalapeno | Bell Pepper
Puff-shroom | Doom-shroom | Boom-shroom
Puff-shroom | Marigold | Penny-shroom
```

Alle vier Ergebnisse sind mit bereits verfügbaren Zutaten portiert. Diese vier Rezepte sind sämtliche Quellenrezepte dieser Ergebnisse. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die bestehenden atomaren Erzeugungs-, Zutatenverbrauchs-, Terrain-, Vorschau- und Markierungswege. Eine eingehende Zutat fusioniert alle passenden Puff-shrooms im Feld; deren Unterfelder und unpassende Pflanzen bleiben erhalten. Kartenkosten und Cooldown bleiben bei der eingehenden Zutat. Insgesamt sind 166 Rezepte aktiv; 502 offene Rezepte für 429 Ergebnisse bleiben. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; das Quellenarchiv bleibt unverändert.

- **Nut-shroom:** 2000 HP, niedriger essbarer Blocker ohne Schussfähigkeit.
- **Bell Pepper:** 300 HP. Ein feindlicher Bodenzombie innerhalb einer Quelleinheit in derselben Reihe startet die originale einsekündige Explosionsanimation. Danach verschwindet die Pflanze und verursacht über den gemeinsamen Feuerreihenadapter 600 Ascheschaden in ihrer Reihe einschließlich Entfernen von Kälteeffekten. Zerquetschen löst sofort aus; gewöhnlicher Tod allein löst keine zusätzliche Explosion aus.
- **Boom-shroom:** 300 HP. Kontakt mit einem feindlichen Bodenzombie oder gewöhnlicher Tod löst einmal aus. Jeder feindliche Bodenzombie derselben Reihe innerhalb einer Quelleinheit, etwa 0,74 Feldern, erhält 1800 Ascheschaden. Kein Krater. Maßgeblich ist die tatsächliche Methode `DoomPuff.Explode`: Der Schleifenaufruf ist virtueller Slot 29, `Zombie.Charred`. Er verursacht einen Treffer je Ziel, keine mit der Gegnerzahl vervielfachte Flächenexplosion. Die Almanachformulierung über wiederholte Explosionen ist entsprechend ungenau.
- **Penny-shroom:** 300 HP. Produziert 100 Geld im Grundintervall von 25 Sekunden mit fünf Prozent Zufallsabweichung; nach 120 aktiven Sekunden 300 Geld. Originale Aufhellung und die halbe Sekunde lange Wachstumsanimation sind eingebunden. Die Produktion erfolgt 0,5 Sekunden nach Beginn der Aufhellung. Zwei beziehungsweise sechs gewöhnliche Goldmünzen bilden die Quellenwerte 100/300 ab; deren nativer Wert und Sammlung überstehen Speichern/Laden ohne flüchtige Sonderzuordnung.

Diese vier Quelltypen erzeugen keine Projektile. Native Puff-Sporen und deren Schusszähler werden ausschließlich für diese Varianten unterdrückt. Die vier Originalprefabs, elf Sprites und sieben Originalclips stammen aus dem lokalen 4.0.5.2-Bundle. Alle Bindungen sind vollständig exportiert; es gibt keine ausgelassenen Spuren. Größe, Schatten, Pivots, Hierarchie und die bestehende Hervorhebung verwenden den vorhandenen Importrenderer. Native Partikel, Audio, Feuerreihendarstellung und Münzen ersetzen entsprechende Quelleneffekte. Es werden keine fremden Unity-Spielskripte geladen.

Die reservierten Speicherpaare sind Puffshroom/Wallnut, Puffshroom/Jalapeno, Puffshroom/Doomshroom und Puffshroom/Marigold. `mStartRow` speichert die Bell-Pepper-Zündphase; Penny-shroom verwendet `mLaunchCounter` für Produktion, `mShootingCounter` für Aufhellung und verzögerte Auszahlung, `mTargetX` für Reife und `mTargetY` für den Wachstumsclip. Alle Zustände verwenden native Simulationsticks und gespeicherte Pflanzenfelder. Pause, Schlaf und Tragen stoppen die Fähigkeiten. Zutatenverbrauch, Rückbau und Board-Ende lösen keine Todesexplosion aus. Laden rekonstruiert Identität und Darstellung ohne Fusionsboni. Spielstände mit neuen Varianten benötigen diese oder eine kompatible neuere Version.

Quellen, Datenwerte, Originalbeschreibungen, vollständige Rezepte, Methoden und Asset-Prüfsummen: `references/nut-shroom/`, `bell-pepper/`, `boom-shroom/`, `penny-shroom/`, `puff-effects/` und `puff-zombie-source/`. Vollständiger Verhaltens- und Speicherbeleg: `references/puff-support/port-notes.md`.

Offline geprüft: produktive Fähigkeiten, beide Rezeptreihenfolgen, Karten- und Handschuhfusionen bei ein bis drei Unterfeldern, Wachstum, Auszahlung, gespeicherte Timer, Zielgrenzen, Tod, Rückbau, Pool-Wiederverwendung und vorhandene Puff-/Scaredy-Regressionen. Originalanimationsvorschauen visuell geprüft; Rezept-/Assetkonsistenz, Workflowprüfungen und Release-Build geprüft. Das sind keine bestätigten Spieltests. Spiel nicht gestartet; Profile und Spielstände unverändert. Version 1.3.0 erhöht die mittlere Nummer für neue Pflanzen. Installation: `references/release-1.3.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.2.1.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove, `workflow.md` und `worklfow.md` bleiben unverändert.

Manuell prüfen: alle vier Rezepte mit Karten und Handschuh in beiden Richtungen; ein, zwei, drei und gemischte Puff-Unterfelder; Kosten, Cooldown, Abbruch, Terrain, Topf/Seerose/Kürbis, Vorschau und Hervorhebung. Nut-shroom mit 2000 HP und ohne Schüsse prüfen. Bell Pepper auf Bodennähe, Zündanimation, 600 Reihenschaden, Kälteentfernung, normalen Tod und Zerquetschen prüfen. Boom-shroom auf engen Kontakt, 1800 Schaden je Ziel, mehrere Ziele, Tod und fehlenden Krater prüfen. Penny-shroom auf 100/300 Geld, Wachstum nach 120 aktiven Sekunden und Münzsammlung prüfen. Schlaf/Kaffee, Pause, Spielgeschwindigkeit, Handschuh, Speichern/Laden während Zündphase, Wachstum, Aufhellung und vor Münzsammlung gegenprüfen.


## Änderung 1.2.1: Puff-Gruppenfusion, Erbsenflug und Größe

Puff-shrooms und ihre vier Schützenvarianten fusionieren jetzt alle passenden Pilze desselben Feldes, auch bei nur einem oder zwei belegten Unterfeldern. Die bisherige Auswahlbeschränkung auf ein Unterfeld verursachte den gemeldeten Fehler. Eine Karte beziehungsweise Handschuhzutat wird einmal verbraucht; nicht passende Pflanzen bleiben erhalten. Atomare Erzeugung und Rückbau bei Fehlern bleiben zuständig. Andere kleine Pflanzen behalten ihre bisherige Unterfeldauswahl.

Die kleinen Erbsen hatten nur zwölf Pixel Abstand zur simulierten Bodenhöhe. Die tatsächliche native Bewegungsmethode erzeugt für Erbsen unter 28 Pixeln sofort einen Bodentreffer. Alle vier Puff-Schützen verwenden jetzt 40 Pixel Flughöhe. Mündungsposition, Originalprojektil, 20 Schaden, Schussfolgen, Rückprall, native Kollision und Speicherung bleiben erhalten.

Die Darstellung richtet sich jetzt nach den tatsächlichen Spritegrenzen und der Preview-Skalierung eines gewöhnlichen nativen Puff-shrooms. Alle sichtbaren Originalsprites der Fusion bestimmen ihre Ausgangsgröße; die einheitliche Skalierung erhält das Seitenverhältnis. Eine fremde Pflanze am Cursor verfälscht die Messung nicht. Die feste Normierung gegen einen 40-Pixel-Schatten entfällt.

Belege, Screenshot, native Methoden und Grenzen: `references/puff-fixes-1.2.1/port-notes.md`. Offline bestanden: 195 Puff-Prüfungen einschließlich erster Flugticks und entfernter Treffer, 171 Unterfeldprüfungen einschließlich teilbelegter Karten-/Handschuhfusionen und Rückbau, 139 Scaredy-Regressionen, 13 Workflowprüfungen, Rezept-/Assetkonsistenz und Release-Build ohne Warnungen oder Fehler. Das sind keine bestätigten Spieltests.

Der Benutzer schloss seine laufende Spielsitzung vor der Installation. Codex hat das Spiel nicht gestartet; Profile und Spielstände bleiben unverändert. Version 1.2.1 erhöht die letzte Nummer für Fixes. Installation: `references/release-1.2.1.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.2.0.dll` nach `Mods/FusionMod.dll` kopieren. Rezeptliste, Quellenarchiv, LawnGlove und beide Workflowdateien bleiben unverändert.

Manuell prüfen: ein, zwei und drei Puff-shrooms pro Feld, Karten- und Handschuhfusion, gemischte Varianten, Größe neben gewöhnlichem Puff-shroom und beide Vorschauen. Alle Schussfolgen, entfernte Ziele, Split-Rückprall, Rüstung, Pause, Terrain und Speichern/Laden gegenprüfen.


## Änderung 1.2.0: Vier Puff-shroom-Fusionen

Pea-shroom, Repeat-shroom, Split Pea-shroom und Gatling Pea-shroom sind vollständig portiert. Alle zehn Quellenrezepte sind registriert, einschließlich der schrittweisen Alternativen aus Pea-shroom und Repeat-shroom. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die bestehende gemeinsame Rezeptauflösung. Die zehn erledigten Zeilen sind aus `fusionen.txt` entfernt. Insgesamt sind 162 Rezepte aktiv; 506 offene Rezepte für 433 Ergebnisse bleiben.

Alle vier Pflanzen besitzen 300 HP und schießen kleine Erbsen mit jeweils 20 Schaden. Das Grundintervall beträgt 2,25 Sekunden mit der ursprünglichen Zufallsabweichung von fünf Prozent. Pea-shroom schießt einmal, Repeat-shroom zweimal, Gatling Pea-shroom viermal. Split Pea-shroom schießt einmal nach vorne und zweimal nach hinten; die hinteren Geschosse prallen am linken Rand einmal zurück und werden dabei entsprechend der Quelle größer. Die Zielsuche reicht über die eigene Reihe; Split Pea-shroom reagiert auch auf Ziele hinter sich. Torchwood verändert diese Geschosse nicht. Alle Fähigkeiten, Originalgeschosse und Schussanimationen sind eingebunden. Es gibt keine zusätzlichen unmittelbaren Fusionsboni.

Die Originalpflanzen und Bullet_puffPea stammen ausschließlich aus dem lokalen 4.0.5.2-Bundle. Die vier Controller, sämtliche acht Clips und alle Sprites sind eingebettet. Zwei nachweislich verwaiste Sichtbarkeitsspuren des Pea-Schussclips werden präzise ausgelassen. Native Trefferpartikel und Audio ersetzen die entsprechenden Quelleneffekte. Die kleinen Pflanzen verwenden weiterhin drei Unterfelder pro Zelle; die Weltgrafik erhält eine 40-Boardpixel-Grundfläche. Native Schlaf-/Kaffee-, Terrain-, Lebenszyklus-, Rüstungs- und Speicherregeln bleiben zuständig.

Speicherpaare: Puffshroom/Peashooter, Puffshroom/Repeater, Puffshroom/Splitpea und Puffshroom/Gatlingpea. Cooldown und Schussphase liegen in `mLaunchCounter` und `mStartRow`. Geschosse speichern ihre Kennung -460010/-460017/-460018 im bei Peas unbenutzten `mCobTargetX`; native Position und Geschwindigkeit erhalten Flugrichtung und Rückprallzustand. Die Darstellung wird nach dem Laden rekonstruiert. Spielstände mit diesen Varianten benötigen diese oder eine kompatible neuere Version.

Quellen, vollständige Rezeptlisten, Beschreibungen, Originaldaten, Asset-Prüfsummen, Schussereignisse, Vorschauen und Portierungsgrenzen stehen unter `references/pea-shroom/`, `repeat-shroom/`, `split-pea-shroom/`, `gatling-pea-shroom/`, `puff-projectile/`, `puff-shooter-base/` und `puff-movement/`. Ausführlicher Verhaltens- und Speicherbeleg: `references/pea-shroom/port-notes.md`.

Offline geprüft: 146 neue Prüfungen des produktiven Pflanzen-, Rezept-, Projektil- und Rückprallcodes, 128 Unterfeldprüfungen einschließlich veröffentlichter Dreierfusionen, Scaredy- und Squash-Regressionen, Workflowprüfungen, Rezept-/Assetkonsistenz und Release-Build. Die vier Originalanimationsvorschauen wurden visuell kontrolliert. Kein Spielstart und keine Änderungen an Profilen oder Spielständen. Installation: `references/release-1.2.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.1.0.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove, `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert. Die mittlere Versionsnummer steigt wegen neuer Pflanzen.

Manuelle Prüfung durch den Benutzer: alle zehn Rezepte mit Karte und Handschuh in beiden Richtungen; Einzel-, Dreier- und gemischte Felder; Kosten, Cooldown, Abbruch, Terrain, Unterlagen, Schlaf/Kaffee und Markierung. Schusszahlen 1/2/3/4, Split-Ziele hinten, zwei Rückprallgeschosse, deren einmalige Größenänderung, 20 Schaden und Torchwood prüfen. Pause, Spielgeschwindigkeit, Tod, Pool-Wiederverwendung und Speichern/Laden während Schussfolge und Rückprall gegenprüfen.


## Änderung 1.1.0: Football Helmet, Football-Nut und Football Gatling

```text
Tall-nut | Football Helmet | Football-nut
Gatling Pea | Football Helmet | Football Gatling
```

Football-Zombies verlieren bei normalem schadenbedingtem Helmverlust mit 50 Prozent Wahrscheinlichkeit einen originalen Football Helmet. Pro Helmverlust wird genau einmal gewürfelt. Instant-Kills, Ascheschaden, bereits helmlose Zombies, freundliche Zombies und Magnetentnahme erzeugen keinen zusätzlichen Drop. Ein gewöhnlicher tödlicher Treffer darf einen Helm hinterlassen; Instant-Kill-Pfade bleiben ausgeschlossen. Die vorhandene Eimerwahrscheinlichkeit und deren Ausschluss tödlicher Treffer bleiben unverändert.

Ein Klick nimmt den losen Helm auf. Ein weiterer Klick pflanzt ihn als niedrige, unbewegliche Pflanze mit 4000 HP oder fusioniert ihn mit Tall-nut beziehungsweise Gatling Pea. Auch beim Magnet-shroom gesammelte FootballHelmet1/2/3 lassen sich anklicken und verwenden. Abbruch, ungültiges Terrain und fehlgeschlagene Erzeugung behalten den Gegenstand. Lose Helme verwenden die bestehende simulierte Gegenstandsbewegung, blinken nach vier Sekunden und verschwinden nach acht aktiven Sekunden; ausgewählte Helme laufen nicht ab. Der Handschuh kann gepflanzte Helme bewegen oder fusionieren. Beide Rezeptreihenfolgen sind registriert. Kosten, Cooldown, Hervorhebung, Vorschau und Unterlagen bleiben in den gemeinsamen Verantwortlichkeiten.

Football-Nut hat 16000 HP und die native hohe Ziel- und Sprungblockade einer Tall-nut. Ein weiterer Football Helmet heilt 5000 HP bis zum Maximum, entsprechend der tatsächlichen Methode `Helmet.Use` und ihrer aus der Quellen-DLL gelesenen Konstante. Football Gatling hat 3000 HP und schießt vier gestaffelte Kugeln im nativen Gatling-Zyklus mit 200 Ticks Grundintervall einschließlich nativer Zufallsverkürzung. Die ersten elf Kugeln verursachen jeweils 80 Schaden plus die aktuellen HP des zweiten Rüstungsteils, mit dem bestehenden Eisenrückstoß um 16 Boardpixel. Jede zwölfte tatsächlich erzeugte Kugel ist die originale rote Football-Kugel. Diese verursacht laut tatsächlicher `Bullet_helmetPea.HitZombie`-Implementierung ausschließlich 80 direkten Schaden, ohne zusätzlichen Eisenschildschaden oder Eisenrückstoß. Sie versetzt feindliche Football-Zombies vor dem Schaden in Flucht. Flüchtende Football-Zombies bleiben feindlich, drehen sich um, laufen nach rechts, fressen keine Pflanzen und verschwinden am rechten Rand ohne Beute. Die Quelle verdoppelt die Geschwindigkeit nur unter drei Quelleinheiten pro Sekunde; diese Grenze wird auf Boardpixel und Simulationsticks umgerechnet. Pause, Kälte, Butter, Gefrieren, Tod und Hypnose bleiben berücksichtigt. Torchwood verändert beide Kugelarten nicht.

Alle Originalpflanzensprites, beschädigten Körper, Controller und Animationen sowie die rote Kugel stammen aus dem lokalen 4.0.5.2-Bundle. Die normale Eisenkugel wird aus dem bestehenden Originalexport wiederverwendet. Pflanzenprefabs: HelmetPlantPrefab 40918 / Transform 116192 / Controller 6196; TallNutFootballPrefab 49272 / 106170 / 6497; HelmetGatlingPrefab 38417 / 107621 / 6294. Rote Kugel: Bullet_helmetPea 34022 / 117795. Die beiden verwaisten Sichtbarkeitsspuren des Gatling-Schussclips entsprechen dem bereits belegten Gatling-Hypno-Fall und werden präzise ausgelassen. Native Effekte ersetzen die originalen Trefferpartikel. Es werden keine Unity-Spielskripte geladen. Wie bei vorhandenen Eisenpflanzen entsteht keine zusätzliche allgemeine Explosionsimmunität: In Replanted beschädigen eigene Cherry-Bomb-Explosionen keine Pflanzen; Jack-in-the-Box und Zerquetschen behalten ihre eigenen Regeln.

Speicherkennungen: Potatomine/GoldMagnet, Tallnut/GoldMagnet und Gatlingpea/GoldMagnet. Identität und HP speichern nativ. Gatling benutzt das bei diesem Typ ungenutzte gespeicherte `mTargetZombieID` als Zähler 0–11; native Gatling-Zielsuche übergibt ihr Ziel separat an `Fire`. Flugkugeln speichern ihre Football-Eisenkennung -450011 beziehungsweise -450168 in `mCobTargetX`; Darstellung und Fähigkeiten werden nach Laden rekonstruiert. Flucht nutzt das bei Football ungenutzte gespeicherte `mSummonCounter` mit -450168 sowie die native Geschwindigkeit. Lose Bodendrops und Cursorwahl bleiben wie vorhandene Eimer flüchtig und verschwinden beim Neuladen des Boards. Gepflanzte Helme und native Magnetgegenstände bleiben gespeichert. Spielstände mit diesen Pflanzen benötigen diese oder eine kompatible neuere Mod-Version.

Quellen, Almanachbeschreibungen, Methoden, Originalassets und statische Aufrufbelege liegen unter `references/football-helmet/`, `football-nut/`, `football-gatling/`, `football-projectiles/`, `football-base/` und `football-native/`. Die Mod enthält jetzt 152 aktive Rezepte; 516 offene Rezepte für 437 Ergebnisse bleiben. Die beiden fertiggestellten Zeilen sind aus `fusionen.txt` entfernt. Football-shroom und Buckshot Commando bleiben offen. `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert. Version 1.1.0 erhöht die mittlere Nummer für neue Pflanzen.

Offline geprüft: 70 neue Prüfungen des produktiven Helm-, Rezept-, Projektil- und Fluchtcodes, 75 Eimerregressionen, 117 Unterfeldprüfungen, 13 Workflowprüfungen, Rezept-/Assetkonsistenz und Release-Build. Die drei Originalanimationsvorschauen wurden visuell kontrolliert. Kein Spielstart, keine Spieltests, keine Änderungen an Profilen oder Spielständen. Installation: `references/release-1.1.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.0.1.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: Helmverlust bei normalen Treffern und Instant-Kills, beide Würfelergebnisse über viele Zombies, Magnetentnahme ohne doppelten Drop, Aufnahme/Abbruch, Standalone-Pflanzung und beide Rezepte mit Karte und Handschuh. Viererschuss, zwölfte rote Kugel, Schildbonus nur bei Eisenkugeln, Rückstoß, Torchwood, Flucht/Umkehr ohne Hypnose und Ausstieg rechts prüfen. Football-Nut mit 16000 HP, Sprungblockade, 5000-HP-Heilung und beschädigten Körpern prüfen. Terrain, Topf/Seerose/Kürbis, Pause, Spielgeschwindigkeit, Vorschau/Markierung, Pool-Wiederverwendung und Speichern/Laden während Schussfolge, fliegenden Kugeln und Flucht gegenprüfen.


## Änderung 1.0.1: Handschuh markiert das gewählte Unterfeld

Die native Hover-Markierung nutzt `Board.ToolHitTest`, während der Handschuh beim Aufnehmen `Board.GetTopPlantAt` aufruft. Deshalb konnte bei korrekt ausgewähltem linken Pilz trotzdem der rechte Pilz leuchten. Der neue, auf den leeren Handschuh begrenzte Postfix für `ToolHitTest` verwendet dieselbe bestehende Unterfeldauswahl wie die Aufnahme. Native Farbgebung bleibt zuständig. Leere Unterfelder markieren keine Geschwisterpflanze. Andere Werkzeuge und getragene Fusionszutaten behalten ihren bisherigen Hit-Test; Aufnahme, Bewegung, Rezepte, Kosten und Speicherung bleiben unverändert.

Der Screenshot `Bildschirmfoto_20261020_115736.png` belegt den visuellen Fehler. Statische Aufrufbelege stehen unter `references/shroom-slots/Board-HighlightPlantsForMouse.asm` und `Board-ToolHitTest.asm`. Offline geprüft: 117 Unterfeldprüfungen einschließlich acht neuer Prüfungen für Hover/Aufnahme, links/rechts/oben, leeres Unterfeld, Werkzeugwechsel und getragene Zutaten; außerdem Workflowprüfungen, Rezept-/Assetkonsistenz und Release-Build ohne Warnungen oder Fehler. Das Spiel wurde nicht gestartet. Die sichtbare Markierung im Spiel prüft der Benutzer.

Installation: `references/release-1.0.1.json`. Sicherung/Rückbau bei geschlossenem Spiel: `backup/FusionMod-1.0.0.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove und beide Workflowdateien bleiben unverändert. Die letzte Versionsnummer steigt, da ausschließlich ein Darstellungsfehler behoben wird.


## Änderung 1.0.0: Drei Pilze pro Feld

Sun-shroom, Puff-shroom und Varianten mit diesen nativen Grundtypen belegen bis zu drei Unterfelder derselben Zelle. Dazu gehört die bereits vorhandene Sundae-shroom-Variante. Die Maus wählt das nächstgelegene Unterfeld: oben mittig, unten rechts oder unten links. Die normale Rastervorschau und die Handschuhvorschau zeigen dieselbe Position wie die anschließende Platzierung. Ein belegtes Unterfeld erlaubt keine weitere gewöhnliche Pflanzung; ein freies Unterfeld bleibt bei einer oder zwei Pflanzen einzeln bepflanzbar. Bestehende, noch mittig stehende Pilze zählen zum oberen Unterfeld und werden beim Laden nicht umsortiert.

Bei drei Pilzen prüft eine eingehende Pflanze alle drei Rezeptidentitäten gegen die ursprüngliche Zutat. Für jedes passende Rezept entsteht ein eigenes Ergebnis im bisherigen Unterfeld. Pflanzen ohne passendes Rezept bleiben erhalten. Bei einer oder zwei Pflanzen wird ausschließlich das gewählte Unterfeld fusioniert. Karte beziehungsweise Handschuh werden pro erfolgreicher Aktion einmal verbraucht; die native Kartenabrechnung und der Handschuh-Cooldown bleiben zuständig. Alle Ergebnisse entstehen vor dem Verbrauch der Zutaten. Schlägt eine Ergebnisallokation fehl, entfernt die Mod die bereits erzeugten Ergebnisse und erhält sämtliche Zutaten.

Der Handschuh hebt ausschließlich die Pflanze im gewählten Unterfeld auf. Er kann diese Pflanze in ein gewähltes freies Unterfeld einer anderen Zelle setzen oder dort passende Fusionen ausführen. Geschwisterpflanzen am Ursprungsfeld bleiben erhalten. Bestehende Abbruch-, Vorschau-, Speicher- und Cooldown-Verantwortlichkeiten bleiben im unveränderten LawnGlove; FusionMod erweitert dessen Auswahl, Platzierung und Rastervorschau über Harmony. Bewegungen innerhalb derselben Zelle bleiben wie bisher ein Abbruch. Seerosen, Töpfe und Kürbisse bleiben erhalten. Imitater-Karten verwenden weiterhin die native Platzierung und werden nicht als neue Unterfeld- oder Fusionsaktion interpretiert.

Die gespeicherten nativen Koordinaten `mX` und `mY` bestimmen das Unterfeld. Es gibt keine neue Speicherkennung, kein zusätzliches Pflanzendatenfeld und keinen flüchtigen Unterfeldkatalog. Geladene Varianten behalten ihre vorhandene Identität. Geskriptete `AddPlant`-Aufrufe und Laden lösen keine Fusion aus. Die Zustandsausnahmen für Terrainprüfungen werden auch bei Fehlern zurückgesetzt. Native Lebenspunkte, Wachstum, Schlaf, Schaden und Sonnenerzeugung bleiben zuständig.

Dieses Release ergänzt Platzierungsverhalten, keine neuen Pflanzen oder Rezepte. Die 150 aktiven Rezepte und die 518 offenen Rezepte für 439 Ergebnisse bleiben unverändert. Insbesondere werden offene Puff-shroom-Fusionen nicht durch erfundene Ersatzrezepte verfügbar. Die Offline-Batchprüfungen setzen ein ausdrücklich isoliertes Testrezept ein, das nicht in der Mod registriert wird. `fusionen.txt`, `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert. Version **1.0.0** erhöht gemäß dem gewünschten Schema die erste Nummer für eine Gameplayänderung; die beiden anderen Nummern beginnen wieder bei null.

Quellenbelege: `references/shroom-slots-source/` enthält die tatsächlichen 4.0.5.2-Methoden von CreatePlant, Mouse, SmallPuff und SunShroom sowie den Puff-shroom-Almanach. Die Dreiecksanordnung wird für native Replanted-Sprites angepasst. Die vom Benutzer verlangte Auswahl per Maus ersetzt die automatische Belegung des ersten freien Quell-Unterfelds. `references/shroom-slots/` dokumentiert native Platzierung, Vorschau, Koordinatenspeicherung und die Portierungsgrenzen.

Offline geprüft: 109 Prüfungen des produktiven Unterfeld-, Karten-, Handschuh-, Vorschau-, Markierungs- und Batchcodes; 75 Bucket-Regressionen; 13 Workflowprüfungen; Rezept-/Assetkonsistenz und Release-Build. Diese Prüfungen laden keinen Spielcode. Das Spiel wurde nicht gestartet; Profile und Spielstände wurden nicht geändert. Installation und binäre Versionsprüfung: `references/release-1.0.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.3.0.dll` nach `Mods/FusionMod.dll` kopieren. Die Vorversion bietet keine Unterfeldauswahl. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer:

- Sun-shroom, Puff-shroom und Sundae-shroom in allen drei Unterfeldern setzen, gemischte Felder prüfen und eine vierte Pflanze ablehnen lassen.
- Maus zwischen den drei Unterfeldern bewegen; Rastervorschau, tatsächliche Position, Vorder-/Hintergrund und gewachsene Sun-shrooms vergleichen. Alte mittig stehende Pilze gegenprüfen.
- Mit dem Handschuh jede einzelne Pflanze aufnehmen, in ein bestimmtes freies Unterfeld einer anderen Zelle setzen, abbrechen und das Ursprungsfeld kontrollieren. Nur die ausgewählte Pflanze darf sich bewegen.
- Terrain, Seerose, Topf, Kürbis, Pause, Tod, Boardwechsel und Speichern/Laden prüfen. Normale Einzelpflanzenfusionen sowie Kosten/Cooldown gegenprüfen.
- Sobald passende Sun-/Puff-Rezepte portiert sind, volle Felder mit drei passenden beziehungsweise gemischt passenden Zutaten prüfen: passende Zutaten fusionieren, unpassende Pflanzen bleiben erhalten. Das Release fügt selbst keine solchen Rezepte hinzu.


## Änderung 0.3.0: Fallout Blover, Solarfruit, Hypno Garlic und Flytrapper

```text
Blover | Chomper | Flytrapper
Blover | Doom-shroom | Fallout Blover
Garlic | Hypno-shroom | Hypno Garlic
Starfruit | Sunflower | Solarfruit
```

Alle Zutaten waren bereits als native Pflanzen vollständig benutzbar. Das lokale Quellenarchiv enthält genau diese vier Rezepte für die vier Ergebnisse. Sämtliche Quellenrezepte sind damit umgesetzt, jeweils in beiden Kartenreihenfolgen und Handschuhrichtungen. Die gemeinsame Auflösung, Vorschau, Markierung, Terrainprüfung und Erzeugung vor dem Zutatenverbrauch bleiben zuständig. Die eingehende Karte bestimmt Kosten und Cooldown. Seerosen, Töpfe und Kürbisse bleiben erhalten. Insgesamt sind 150 Rezepte aktiv; 518 Rezepte für 439 Ergebnisse bleiben offen. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; Folgefusionen und `fusionen-quellen.json` bleiben unverändert.

- **Fallout Blover:** verursacht weltweit 180 Ascheschaden pro Impuls, auch gegen fliegende Ziele. Die tatsächliche Quellenmethode erzeugt bereits beim ursprünglichen Blasereignis nach etwa 0,49 Sekunden einen Impuls. Danach folgen über zehn Sekunden zehn weitere Impulse im Sekundentakt; anschließend verschwindet die Pflanze. Das sind elf Impulse einschließlich des ersten Ereignisses. Freundliche, bereits sterbende und ungültige Ziele sowie Boss, Grabstein und Zielobjekt bleiben ausgeschlossen. Die geerbte Blasereaktion vertreibt Nebel für 30 Sekunden. Ballonzombies erhalten Schaden, werden aber nicht zusätzlich durch die Wirkung eines gewöhnlichen Blovers entfernt. Es entstehen weder Krater noch zusätzliche Geschosse.
- **Solarfruit:** schießt ausschließlich nach einem normalen Klick fünf originale Sterne in die Richtungen 30, -30, 180, 90 und -90 Grad. Jeder Stern verursacht 100 Schaden. Ein erfolgreicher Angriff kostet 25 Sonne. Beide tatsächlichen Quelleneinstiege verlangen dabei strikt mehr als 25 vorhandene Sonne; genau 25 Sonne genügt deshalb nicht. Der Originalcontroller spielt den Schussclip mit Geschwindigkeit zwei; das Schussereignis liegt nach etwa 0,21 Sekunden. Mit einem Starry Pumpkin in derselben Zelle erfolgt alle 0,35 Sekunden ein bezahlter Angriff. Diese Bedingung ist implementiert und offline geprüft; Starry Pumpkin selbst ist weiterhin eine offene, eigenständige Fusion und wird durch dieses Release nicht verfügbar. Ohne diesen Kürbis gibt es keine automatische Zielsuche oder automatische Schüsse. Werkzeug-, Handschuh- und Loslassklicks lösen keinen Angriff aus.
- **Hypno Garlic:** lenkt alle 0,5 aktiven Sekunden geeignete freundliche Bodenzombies innerhalb einer Quelleinheit in derselben Reihe um. Ein feindlicher Biss erzeugt zuerst einen frischen gewöhnlichen Garlic mit 300 HP, verbraucht erst danach die Variante, hypnotisiert den Beißer und löst dessen Reihenwechsel aus. Die native Reihenwahl erhält gültige Reihen, Wasser-/Landverträglichkeit, Bewegung und Speicherung. Ein laufender Reihenwechsel wird nicht neu gestartet. Schlägt die Erzeugung des Garlic fehl, bleiben die Variante und der feindliche Zombie unverändert. Beschädigte Originalkörper und Hypnokappen sind eingebettet.
- **Flytrapper:** schwebt mit seinen Originalanimationen, wird von der nativen Auswahl für Bodenangriffe ausgeschlossen und kann nahe Boden- und Luftziele verschlingen. Der tatsächliche Quellenradius beträgt eine Quelleinheit vor dem versetzten Suchursprung. Native Chomper-KI übernimmt Biss, erfolglose Bisse gegen große Ziele, Verschlingen, 40 Sekunden Verdauung und Speicherung. Fahrzeuge, Bungee, Boss, Grabstein und Zielobjekt sind keine Schluckziele; freundliche Ziele werden nicht angegriffen.

Alle vier Pflanzen besitzen 300 HP. Die reservierten gespeicherten Paare sind `Blover/Doomshroom`, `Starfruit/Sunflower`, `Garlic/Hypnoshroom` und `Chomper/Blover`. Aktive Timer, Fusionsphasen und Impulszahl liegen in nativen gespeicherten Pflanzenfeldern. Solarsterne behalten ihre 100 Schaden durch die gespeicherte Kennung -440100 in `Projectile.mCobTargetX`, auch nach dem Laden fliegender Geschosse. Normale Sterne behalten ihren normalen Schaden. Native Sternbewegung, Rüstung, Kollision, Lebensdauer und die bereits importierte Originalsterngrafik bleiben zuständig. Pause, Tragen, Bungee, Tod und Board-Abbau stoppen die neuen Fähigkeiten. Animationen erzeugen keine zusätzlichen Spielereignisse.

Originalgrafik und alle zehn Pflanzenclips stammen ausschließlich aus dem lokalen `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: DoomBloverPrefab 42486 / Transform 119636 / Controller 6177; SunStarPrefab 52553 / 106215 / 6485; HypnoGarlicPrefab 43588 / 108762 / 6291; ChomperBloverPrefab 53765 / 100920 / 6240. Die Sterne verwenden das bereits eingebettete Originalobjekt Bullet_star 33973. SunStar besitzt im Original keinen sichtbaren Schatten; sein Körper liefert den Grafikanker. Die drei nachweislich verwaisten Transformspuren des Hypno-Garlic-Clips werden präzise dokumentiert und ausgelassen. Native Wolkenpartikel ersetzen die ursprünglichen Fallout-Partikel. Audio, Hypnose und Reihenbewegung bleiben nativ; Unity-Spielskripte werden nicht geladen.

Quellenrezepte, Beschreibungen, Methoden, Scriptidentitäten, Exportprüfsummen und Offlinevorschauen stehen in den vier Ergebnisordnern unter `references/`. Gemeinsamer Verhaltens- und Speicherbeleg: `references/fallout-blover/port-notes.md`; Quellenbasisklassen und Konstanten: `references/support-source-base/`; Nebelquelle: `references/fallout-fog-source/`; native Aufruf- und Speicherwege: `references/support-native/` und die ergänzenden `support-native-garlic*`-Ordner. Der Workflow einschließlich `worklfow.md` bleibt unverändert.

Offline geprüft: Rezept-/Assetkonsistenz, 13 Workflowprüfungen, sämtliche 18 C#-Testsuiten einschließlich 92 neuer Fähigkeitsprüfungen, alle zehn Originalclips und Release-Build. Dies bestätigt keine Spieltests. Installation: `references/release-0.3.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.2.0.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen 0.3.0 oder eine kompatible neuere Version. LawnGlove bleibt unverändert. Die mittlere Versionsnummer steigt wegen neuer Pflanzen; die Patchnummer beginnt wieder bei null.

Manuell durch den Benutzer prüfen: alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen; Kosten/Cooldown, Abbruch, Markierung, Dach/Wasser und Unterlagen. Fallouts Startimpuls, zehn Folgeimpulse, Luftziele, Rüstung, Nebel und Ablauf prüfen. Solarfruits Klick, Grenze bei 25/26 Sonne, fünf Richtungen, 100 Schaden und gespeicherte Sterne prüfen. Hypno Garlic mit feindlichen Bissen und freundlichen Kontakten, Reihenrändern und Wassergrenzen prüfen. Flytrapper mit Boden-, Ballon- und großen Zielen, Verdauung und fehlender Bodenangreifbarkeit prüfen. Originalanimationen, Pause, Handschuh, Bungee, Tod und Speichern/Laden mitten in sämtlichen Phasen prüfen. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert.

## Änderung 0.2.0: Hypno Pea, Hypno Repeater, Split Hypno und Gatling Hypno

```text
Gatling Pea | Hypno-shroom | Gatling Hypno
Hypno Pea | Peashooter | Hypno Repeater
Hypno Pea | Repeater | Split Hypno
Hypno Pea | Split Pea | Gatling Hypno
Hypno Repeater | Peashooter | Split Hypno
Hypno Repeater | Repeater | Gatling Hypno
Hypno-shroom | Peashooter | Hypno Pea
Hypno-shroom | Repeater | Hypno Repeater
Hypno-shroom | Split Pea | Split Hypno
Peashooter | Split Hypno | Gatling Hypno
```

Alle vier Ergebnisse besitzen einen direkten Rezeptweg mit bereits vollständig benutzbaren nativen Zutaten. Die zusätzlichen Wege werden in der Reihenfolge Hypno Pea, Hypno Repeater, Split Hypno und Gatling Hypno verfügbar. Damit sind alle zehn Quellenrezepte dieser Ergebnisse umgesetzt. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die vorhandene gemeinsame Auflösung, Terrainprüfung und Erzeugung vor dem Zutatenverbrauch. Die eingehende Karte bestimmt Kosten und Cooldown. Seerosen, Töpfe und Kürbisse bleiben erhalten. Insgesamt sind 146 Rezepte aktiv; 522 Rezepte für 443 Ergebnisse bleiben offen. Die zehn erledigten Zeilen sind aus `fusionen.txt` entfernt; Folgefusionen und `fusionen-quellen.json` bleiben erhalten.

- **Hypno Pea:** schießt eine originale Hypnoerbse mit 30 Schaden pro 1,5 Sekunden.
- **Hypno Repeater:** schießt zwei Hypnoerbsen mit je 30 Schaden pro 1,5 Sekunden. Nach 15 aktiven Sekunden kann ein normaler Klick die Pflanze kostenlos zu Split Hypno weiterentwickeln.
- **Split Hypno:** schießt eine Hypnoerbse nach vorn und zwei nach hinten; jede verursacht 30 Schaden. Die hinteren Geschosse prallen am linken Bildschirmrand ab. Ein Ziel auf einer Seite aktiviert beide Köpfe. Nach 15 aktiven Sekunden entwickelt ein normaler Klick die Pflanze kostenlos zu Gatling Hypno weiter.
- **Gatling Hypno:** schießt vier Hypnoerbsen mit je 30 Schaden pro 1,5 Sekunden.

Ein tödlicher Hypnoerbsentreffer erzeugt an der Position und in der Reihe des besiegten Zombies genau einen freundlichen PeaHead-Zombie mit 270 HP. Dieser schießt alle 1,5 Sekunden originale Hypnoerbsen mit 20 Schaden. Auch seine tödlichen Geschosse können weitere freundliche PeaHead-Zombies erzeugen; die tatsächliche Quellenmethode wählt denselben Geschosstyp 153. Der erste Schuss wartet entsprechend `PeaShooterZ.Awake` zufällig 0,5 bis 1,5 Sekunden. Überlebende, reine Rüstungstreffer, bereits sterbende oder freundliche Ziele sowie Boss, Grabstein und Zielobjekt erzeugen keine Verbündeten. Normales Torchwood verändert Hypnoerbsen nicht. Die besondere Feuererbse gehört zur eigenständigen Hypnowood-Aufwertung und ist kein Geschoss dieser vier Grundpflanzen.

Alle Pflanzen besitzen 300 HP. Die gespeicherten Paare sind `Peashooter/Hypnoshroom`, `Repeater/Hypnoshroom`, `Splitpea/Hypnoshroom` und `Gatlingpea/Hypnoshroom`. Native Shooter-KI steuert Zielauswahl, Reihen, Schusszahl und versetzte Salven; die vorhandene gemeinsame Wachstumslogik speichert die 15 Sekunden in `Plant.mStartRow`. Pause, Handschuh, Bungee und Tod stoppen das Wachstum. Weiterentwicklung übernimmt verbleibende HP und erzeugt keine Fusionsbelohnung. Geschossherkunft und 30 beziehungsweise 20 Schaden bleiben in `Projectile.mCobTargetX` gespeichert. Die besondere Munition der Verbündeten wird in deren unbenutztem, nativ gespeichertem `mSummonCounter` markiert. Rückprall und Sonderwirkung funktionieren auch nach Laden bereits fliegender hinterer Geschosse. Es entstehen keine zusätzlichen flüchtigen Herkunftstabellen.

Pflanzen- und Geschossgrafik sowie alle acht Originalclips stammen aus dem lokalen `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: HypnoPeashooterPrefab 37181 / Transform 120499 / Controller 6360; HypnoRepeaterPrefab 54444 / 110839 / 6267; HypnoSplitPrefab 37189 / 120507 / 6363; GatlingPeaPrefab 38424 / 107628 / 6294; Bullet_hypnoPea 63610 / 114455. Der irreführend benannte GatlingPeaPrefab besitzt tatsächlich das Script HypnoPeashooter. SuperHypnoGatlingPrefab 38420 gehört zu einer anderen Aufwertung und wird nicht verwendet. Originalhierarchien, Pivots, Schatten und additive Animationen bleiben erhalten. Nachweislich verwaiste Controllerbindungen sind einzeln dokumentiert. Beschworene Zombies verwenden die native PeaHead-Darstellung, Laufanimation, Hypnose, Biss-, Rüstungs- und Todeslogik; ihre Geschosse verwenden die Originalgrafik. Trefferpartikel und Audio bleiben nativ. Unity-Spielskripte werden nicht übernommen.

Quellenrezepte, Originalbeschreibungen, Methoden, Scriptidentitäten, Exportprüfsummen und Offlinevorschauen stehen unter `references/hypno-pea/`, `references/hypno-repeater/`, `references/split-hypno/`, `references/gatling-hypno/` und `references/hypno-projectile/`. Gemeinsamer Verhaltensbeleg: `references/hypno-pea/port-notes.md`; Quellenbasisklassen: `references/hypno-source-base/` und `references/hypno-torch/`; native Aufruf- und Speicherwege: `references/hypno-native/`. Insbesondere enthält `Zombie.UpdateActions` eine eingebettete Kopie der PeaHead-Schusslogik. Beide tatsächlichen Schusswege sind berücksichtigt. Der Workflow einschließlich `worklfow.md` bleibt unverändert.

Offline geprüft: Rezept-/Assetkonsistenz, 13 Workflowprüfungen, sämtliche 17 C#-Testsuiten einschließlich 112 neuer Hypno-Prüfungen, Originalanimationsvorschauen und Release-Build. Dies bestätigt keine Spieltests. Installation: `references/release-0.2.0.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.61.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen 0.2.0 oder eine kompatible neuere Version. LawnGlove bleibt unverändert. Die mittlere Versionsnummer steigt wegen neuer Pflanzen; die Patchnummer beginnt wieder bei null.

Manuell durch den Benutzer prüfen: alle zehn Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen; Kosten/Cooldown, Abbruch, Markierung, Dach/Wasser und Unterlagen. Schusszahlen und beide Split-Köpfe; Rückprall am linken Rand; 30 Schaden, Rüstung und tödliche Treffer; Position, 270 HP, Hypnose und 20-Schaden-Geschosse der Verbündeten einschließlich weiterer Beschwörungen. Beide Klick-Upgrades nach 15 Sekunden, beschädigte Pflanzen und Abbruch mit Handschuh prüfen. Originalanimationen, Torchwood, Pause, Bungee, Tod sowie Speichern/Laden während Salven, Wachstum, Geschossflug und nach Beschwörungen prüfen. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert.

## Änderung 0.1.61: Sunny-shroom, Starry-shroom, Sturdy-shroom und Camo-shroom

```text
Blover | Scaredy-shroom | Camo-shroom
Scaredy-shroom | Starfruit | Starry-shroom
Scaredy-shroom | Sunflower | Sunny-shroom
Scaredy-shroom | Wall-nut | Sturdy-shroom
```

Sämtliche Zutaten waren bereits als native Pflanzen vollständig benutzbar. Das lokale Quellenarchiv enthält genau diese vier Rezepte für die vier Ergebnisse; es gibt keine weiteren Alternativen. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die gemeinsame Rezeptauflösung, Terrainprüfung und Erzeugung vor dem Zutatenverbrauch. Kosten und Cooldown stammen von der eingehenden Karte. Seerosen, Töpfe und Kürbisse bleiben erhalten. Insgesamt sind 136 Rezepte aktiv; 532 Rezepte für 447 Ergebnisse bleiben offen. Die vier erledigten Einträge sind aus `fusionen.txt` entfernt. Folgefusionen und `fusionen-quellen.json` bleiben unverändert.

- **Sunny-shroom:** produziert jeweils 25 Sonne und produziert auch während der Angst weiter. Das Produktionsintervall beginnt bei 25 Sekunden, sinkt kontinuierlich um eine halbe Sekunde pro aktiver Sekunde und erreicht mindestens zehn Sekunden. Tatsächlich setzt `ScaredySun.GetNearZombies` das Intervall bei nahen Zombies fortlaufend auf 25 Sekunden zurück. Ein bereits laufender Produktionsvorlauf wird dabei nicht neu gestartet. Der erste Vorlauf beträgt vier bis sieben Sekunden, weitere Vorläufe verwenden das aktuelle Intervall mit fünf Prozent Zufall. Die originale Aufhellung steigt innerhalb von drei Sekunden von eins auf vier; am Maximum entsteht die Sonne. Anschließend klingt sie innerhalb von 0,6 Sekunden ab.
- **Starry-shroom:** erzeugt ohne Zielvoraussetzung alle 0,75 Sekunden fünf originale Sterne mit je 20 Schaden. Die Richtungen sind 30, -30, 180, 90 und -90 Grad. Nahe Zombies unterbrechen den Angriff und lösen die ursprüngliche Angstfolge aus. Das Schussereignis liegt bei 0,25 Sekunden im 0,416667 Sekunden langen Originalclip. Native Sternbewegung, Reihenwechsel, Kollision, Rüstung, Lebensdauer und Speicherung bleiben zuständig. Der vorhandene Originalstern `Bullet_star` wird wiederverwendet; die Quellenmethode erzeugt ausdrücklich Geschosstyp 39.
- **Sturdy-shroom:** besitzt 4000 HP und erhält während des Duckens nur die Hälfte des eingehenden HP-Schadens. Bisse, Zombieerbsen, Basketballs und tödliche Bisse sind berücksichtigt; ganzzahliger Schaden wird wie in der Quelle halbiert. Ein zunächst tödlicher Biss darf durch die Halbierung überlebt werden und erhöht dann nicht den Zähler gefressener Pflanzen. Native Jack-in-the-Box-Entfernung wird in 1800 Schaden beziehungsweise 900 Schaden beim Ducken übersetzt. Nach mehr als zehn Sekunden ununterbrochener Bedrohung richtet sich die Pflanze auf. Nach weiteren 0,5 Sekunden verursacht sie in der ursprünglichen Umgebung 20 Schaden an Schild und Körper und stößt geeignete Ziele um zwei Quelleinheiten zurück. Fünf Sekunden lang tritt keine neue Angst ein. Quetschen und gewöhnliches Entfernen behalten ihre nativen Todeswege.
- **Camo-shroom:** schwebt mit seiner Originalanimation und greift nicht an. Die eigene fliegende Identität wird aus der nativen Auswahl für Bodenangriffe ausgeschlossen. In einer Umgebung von 3×3 Zellen verhindert ein aktiver Camo-shroom Bisse auf duckende Scaredy-shrooms, einschließlich der nativen Grundpflanze und aller portierten Varianten. Der Schutz endet beim Aufrichten oder wenn der Camo-shroom schläft, getragen wird oder verschwindet. Der Schutz benachbarter Pflanzen verhindert ausschließlich Beißen; andere Angriffsarten bleiben möglich.

Sunny, Starry und Camo besitzen 300 HP. Alle vier Varianten behalten den nativen Pilz-, Kaffee-, Terrain- und Handschuhlebenszyklus. Die reservierten gespeicherten Paare sind `Scaredyshroom/Sunflower`, `Scaredyshroom/Starfruit`, `Scaredyshroom/Wallnut` und `Scaredyshroom/Blover`. Produktionsvorlauf, halbe Intervallticks, Aufhellung, Angstphase, verzögerter Abwehrstoß und Angstimmunität liegen ausschließlich in nativen gespeicherten Feldern. Bereits fliegende Sterne benötigen keine zusätzliche flüchtige Herkunftsverwaltung. Pause, Schlaf, Tragen, Bungee und Tod stoppen die Simulation; Originalanimationen erzeugen keine zusätzlichen Effekte. Karten- oder Handschuhvorschauen erhalten die Originalgrafik über den vorhandenen Import-Renderer.

Alle Originalgrafiken und 14 Clips stammen ausschließlich aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: ScaredySunPrefab 75230 / Transform 100428 / Controller 6420; ScaredyStarPrefab 75229 / 100427 / 6424; ScaredyNutPrefab 75226 / 100424 / 6422; ScaredyBloverPrefab 37300 / 91125 / 6421. Sterne verwenden das bereits eingebettete Originalobjekt 33973. Unity-Spielskripte werden nicht geladen. Quellenrezepte, Almanachbeschreibungen, tatsächliche Methoden, Scriptidentitäten, Exportprüfsummen und Offlinevorschauen stehen in den vier Ergebnisordnern unter `references/`. Gemeinsamer Verhaltens- und Speicherbeleg: `references/sunny-shroom/port-notes.md`; native Aufrufer: `references/scaredy-support-native/`. Der Workflow einschließlich `worklfow.md` bleibt unverändert.

Offline geprüft: sämtliche 16 bestehenden C#-Testsuiten, darunter 139 Scaredy-Prüfungen mit den neuen Fähigkeiten, tatsächlichen Schadenspfaden, verschachtelten Schadenskontexten, Pool-Wiederverwendung und Wiederaufnahme gespeicherter Phasen; Rezept-/Assetkonsistenz, 13 Workflowprüfungen, alle 14 neuen Originalclips und Release-Build. Keine bestätigten Spieltests. Installation: `references/release-0.1.61.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.60.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen 0.1.61 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuell durch den Benutzer prüfen: beide Kartenreihenfolgen und Handschuhrichtungen, Kosten/Cooldown, Abbruch, Markierung, Wasser/Dach, Unterlagen und Kürbisse; Sunnys schneller werdende Produktion und fortgesetzte Produktion beim Ducken; Starrys fünf Richtungen und Angstunterbrechung; Sturdys Schadenshalbierung, tödliche Bisse, Jack-Explosionen, verzögerten Abwehrstoß und fünf Sekunden Angstimmunität; Camos Flug und Beißschutz innerhalb und außerhalb der 3×3-Umgebung. Originalanimationen, Rüstung, Pause, Schlaf/Kaffee, Handschuh, Bungee, Tod und Speichern/Laden mitten in Aufhellung, Schuss, Ducken oder Abwehrstoß prüfen. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert.

## Änderung 0.1.60: Speedy-shroom, Blasty-shroom, Shivery-shroom und Spuddy-shroom

```text
Peashooter | Scaredy-shroom | Speedy-shroom
Cherry Bomb | Scaredy-shroom | Blasty-shroom
Ice-shroom | Scaredy-shroom | Shivery-shroom
Potato Mine | Scaredy-shroom | Spuddy-shroom
```

Alle vier Zutatenpaare waren vorher vollständig benutzbar. Das lokale Quellenarchiv enthält genau diese vier Rezepte für diese Ergebnisse; sämtliche Alternativen sind damit abgedeckt. Beide Kartenreihenfolgen und Handschuhrichtungen nutzen die vorhandene gemeinsame Rezeptauflösung, Terrainprüfung und Erzeugung vor dem Zutatenverbrauch. Kosten und Cooldown stammen von der eingehenden Karte. Seerosen, Töpfe und Kürbisse bleiben erhalten. Insgesamt sind 132 Rezepte aktiv; 536 Rezepte für 451 Ergebnisse bleiben offen. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; `fusionen-quellen.json` und Folgefusionen bleiben unverändert.

- **Speedy-shroom:** je Schuss fünf Prozent mehr Angriffstempo und ein zusätzlicher Schadenspunkt, höchstens 150 Prozent und 50 Schaden. Das Grundintervall beträgt 1,5 Sekunden und sinkt bis 0,6 Sekunden. Angst setzt den Bonus zurück. Fliegende Erbsen behalten ihren Schaden. Normales Torchwood addiert 20 Schaden und erhält den nativen Flächenschaden.
- **Blasty-shroom:** eine originale Kirschspore mit 40 Direktschaden alle 1,5 Sekunden. Ducken erzeugt einmalig 600 Kirschschaden im Originalradius. Der Effekt liegt bei 0,833333 Sekunden im Originalclip; dauerhaftes Ducken wiederholt ihn nicht. Die allgemeine Kirschwirkung beim Verbrauch einer Cherry Bomb bleibt erhalten.
- **Shivery-shroom:** ein originales Eisgeschoss mit 40 Schaden alle 1,5 Sekunden, Kälte, zehn Gefrierpunkten und vierfachem Schaden gegen gefrorene Ziele. Beim Ducken erhalten nahe Zombies 20 Schaden und vier Sekunden Gefrieren. Native Immunitäten, längere bestehende Gefrierzeiten und Torchwood-Auftauen bleiben erhalten.
- **Spuddy-shroom:** eine originale Kartoffelspore alle drei Sekunden. In den ersten 20 aktiven Sekunden verursacht sie 600 Schaden und kostet die Pflanze 75 HP; danach verursacht sie 60 Schaden ohne HP-Verbrauch. Ohne Heilung tötet der vierte frühe Schuss die 300-HP-Pflanze; die letzte Spore fliegt weiter. Tod erzeugt eine Kartoffelexplosion mit 600 Schaden und dem Originalradius einer Quelleinheit. Zutatenverbrauch und Board-Abbau lösen keine Todesexplosion aus.

Alle vier Pflanzen besitzen 300 HP und behalten native Pilz-, Kaffee-, Terrain- und Handschuhregeln. Die gespeicherten Kennungen lauten `Scaredyshroom/Peashooter`, `Scaredyshroom/Cherrybomb`, `Scaredyshroom/Iceshroom` und `Scaredyshroom/Potatomine`. Native Felder speichern Bonus, Vorlauf, Schussphase, Ducken und Aufrichten. Auch bereits fliegende Geschosse behalten beim Laden ihren Schaden, ihre Sonderwirkung und die Originalgrafik über eine reservierte Kennung in `Projectile.mCobTargetX`. Die bestehenden Gefrierpunkte auf Zombies bleiben flüchtig.

Originalgrafik und alle 20 Pflanzenclips stammen aus dem lokalen `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: PeaScaredyPrefab 75222 / Transform 100420 / Controller 6418; CherryScaredyPrefab 75219 / 100417 / 6416; IceScaredyShroomPrefab 75227 / 100425 / 6612; ScaredyPotatoPrefab 75220 / 100418 / 6419. Originalgeschosse sind Bullet_pea 33962, Bullet_puff_cherry 73654, Bullet_snowPea 62921 und Bullet_puffPotato 73652. Alle fünf Clips je Pflanze, Originalhierarchie, Pivots, Schatten und Alphakurven sind eingebettet. Treffer-, Eis- und Explosionspartikel sowie Audio verwenden native Effekte. Unity-Spielskripte werden nicht geladen.

Quellen, Beschreibungen, Scriptidentitäten, Methodenauszüge, Assetprüfsummen und Offlinevorschauen stehen in den vier Ergebnisordnern unter `references/`. Der gemeinsame Beleg steht in `references/speedy-shroom/port-notes.md`; native Aufrufer und Serialisierung in `references/scaredy-native/`. Der bestehende Workflow einschließlich `worklfow.md` bleibt unverändert.

Offlineprüfung: Rezept-/Assetkonsistenz, 13 Workflowprüfungen, gezielte Scaredy-Regressionen, betroffene bestehende Regressionen, Originalanimationen und Release-Build. Keine bestätigten Spieltests. Installation: `references/release-0.1.60.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.59.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen 0.1.60 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuell durch den Benutzer prüfen: alle vier Rezepte und beide Richtungen, Kosten/Cooldown, Abbruch, Markierung, Wasser/Dach und Unterlagen. Speedys Steigerung und Angstreset; Blastys einmalige Explosion; Shiverys Geschosse und lokale Gefrierwirkung; Spuddys HP-Verbrauch, 20-Sekunden-Grenze, letzter Schuss und Todesexplosion. Originalanimationen, Torchwood, Rüstung, Pause, Schlaf/Kaffee, Handschuh, Bungee und Speichern/Laden mitten in Fähigkeiten und Geschossflug prüfen. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert.

## Änderung 0.1.59: Black Cherry, Frozen Cherry, Demise-shroom und Sundae-shroom

```text
Cherry Bomb | Doom-shroom | Black Cherry
Cherry Bomb | Ice-shroom | Frozen Cherry
Doom-shroom | Ice-shroom | Demise-shroom
Ice-shroom | Sunflower | Sundae-shroom
```

Das lokale Quellenarchiv enthält genau diese vier Rezepte für die vier Ergebnisse. Alle Zutaten waren vorher vollständig benutzbar. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die gemeinsame Rezeptauflösung, Terrainprüfung und Erzeugung vor Zutatenverbrauch. Kosten und Cooldown stammen von der eingehenden Karte. Unterlagen und Kürbisse bleiben beim Fusionieren erhalten. Insgesamt sind 128 Rezepte aktiv; 540 Rezepte für 455 Ergebnisse bleiben offen. Die vier erledigten Einträge sind aus `fusionen.txt` entfernt; Folgefusionen und `fusionen-quellen.json` bleiben unverändert.

- **Black Cherry:** Die erfolgreiche Fusion erzeugt einmalig eine Kirschexplosion mit 1800 Ascheschaden und zählt jeden beschädigten Zombie, einschließlich Überlebender. Nach 1,5 Sekunden folgt eine Doom-Explosion mit 1800 + 300 Schaden pro zuvor beschädigtem Zombie. Vorzeitiges Zerquetschen löst die Folgeexplosion ebenfalls aus; gewöhnliches Entfernen wiederholt sie nicht. Die Doom-Explosion besitzt den Quellenradius von fünf Welteinheiten und hinterlässt den vorhandenen nativen 180-Sekunden-Krater. Der Krater entfernt wie bisher den Pflanzenstapel seiner Zelle. Laden wiederholt die erste Explosion nicht.
- **Frozen Cherry:** bleibt stehen und absorbiert geeignete freundliche Kirschgeschosse auf gleicher Höhe und Reihe. Pinke Kirschen liefern einen Punkt, Feuerkirschen zwei, explosive Kirschen zehn. Ab 100 Punkten entsteht eine native aufnehmbare Cherry-Bomb-Karte; der Quellenzähler wird auf null zurückgesetzt. Normale Erbsen und fremde Geschosstypen bleiben unberührt. Tod erzeugt 1800 Kirschschaden und zehn Sekunden Gefrieren im Explosionsradius. Der Quellenradius beträgt zwei Welteinheiten, rund 118,52 Replanted-Pixel. Die generelle Kirschwirkung beim Verbrauch einer Cherry Bomb bleibt erhalten.
- **Demise-shroom:** verwendet den originalen Explosionsclip mit Geschwindigkeit 0,7; dessen Ereignis liegt bei 2/3 Clipsekunden, also nach rund 0,96 Simulationssekunden. Danach erhalten geeignete Zombies auf dem gesamten Brett 1800 Ascheschaden und fünf Sekunden Gefrieren. Der native Krater bleibt 180 Sekunden. Reguläre Wellenerzeugung und deren Zeitgeber pausieren zehn Sekunden. Vorhandene Zombies und Pflanzen bewegen beziehungsweise handeln weiter; geskriptete Erzeugung durch andere Mods bleibt zuständig.
- **Sundae-shroom:** erzeugt dreimal jeweils 25 Sonne. Bereits vor der jeweiligen Eiswirkung gefrorene Zombies lassen zusätzlich je 15 Sonne fallen. Die ersten zwei Eiswirkungen verursachen je 20 Schaden und Kälte ohne neues Gefrieren. Die dritte verursacht ebenfalls 20 Schaden, friert vier Sekunden und entfernt die Pflanze. Der erste Produktionsvorlauf beginnt nach vier bis sieben Sekunden, weitere Vorläufe nach 25 Sekunden mit fünf Prozent Zufall. Originalaufhellung von Faktor eins auf vier und anschließendes Abdunkeln sind enthalten; die Sonne entsteht am Helligkeitsmaximum.

Alle vier Ergebnisse besitzen 300 HP. Native Terrain-, Pilz-/Kaffee-, Handschuh-, Pause- und Bungee-Regeln bleiben erhalten. Frozen Cherry verwendet die gewöhnliche gespeicherte Basis `Peashooter/Iceshroom`, damit die dauerhafte Pflanze einschließlich Fahrzeugkontakt regulär angreifbar bleibt. Sundae-shroom verwendet `Sunshroom/Iceshroom` für denselben normalen Pilzlebenszyklus. Black Cherry verwendet `Cherrybomb/Doomshroom`, Demise-shroom `Doomshroom/Iceshroom`. `mLaunchCounter`, `mStartRow`, `mTargetX` und `mTargetY` speichern Vorlauf, Produktionsstufe, Treffer-/Absorptionszähler und Aufhellungsphase. Native Spezial- und Schussereignisse erzeugen keine zusätzlichen Effekte. Native Freeze-Immunitäten und längere bestehende Gefrierzeiten bleiben erhalten.

Die zehn Sekunden Spawnsperre von Demise-shroom liegen in den für Krater unbenutzten und nativ gespeicherten Feldern `GridItem.mSeedType = Iceshroom` und `mSunCount`; Reset entfernt beide. Mehrere Sperren wirken gleichzeitig, ihre Zeiten addieren sich nicht. Die Geschosskennung pinker und explosiver Kirschen liegt jetzt in `Projectile.mCobTargetX` bei den exklusiven Werten `-41001` und `-41010`. Native Initialisierung, Reset und Tod bereinigen sie. Dadurch bleiben Schaden, Originalgrafik und Absorbierbarkeit bereits fliegender Kirschgeschosse nach dem Laden erhalten. Die vorhandenen übrigen flüchtigen Zombieeffekte und Sonnenwert-Helfer behalten ihre bisherigen Speichergrenzen.

Originalgrafik und alle sechs Clips stammen ausschließlich aus dem lokalen `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: DoomCherryPrefab 37633 / Transform 119536 / Controller 6234; IceCherryPrefab 66980 / 113301 / 6235; IceDoomPrefab 39392 / 113189 / 6606; SunIceShroomPrefab 55289 / 92994 / 6315. Black Cherry behält die additive Schüttelebene mit Gewicht eins. Kein unbekannter Animationspfad wird verworfen. Diese vier Pflanzen erzeugen keine neuen fliegenden Projektile. Frozen Cherry verwendet die bereits portierten Kirschgeschosse und die native Kartenaufnahme; Explosionen, Kältepartikel und Geräusche verwenden vorhandene native Effekte. Unity-Spielskripte werden nicht ausgeführt.

Quellenrezepte, Almanachtexte, Prefab-/Scriptidentitäten, tatsächliche Methoden und Exportprüfsummen stehen in `references/black-cherry/`, `frozen-cherry/`, `demise-shroom/` und `sundae-shroom/`. Gemeinsamer Verhaltens- und Speicherbeleg: `references/black-cherry/port-notes.md`. Produktionsbelege: `references/sundae-production/` und `ice-producer-base/`. Native Aufrufer und Serialisierung: `references/ice-instant-native/`. Der Recherchehelfer kann jetzt gleichnamige verschachtelte Coroutinen über ihren tatsächlichen Besitzer auswählen. Der Workflow ergänzt ausschließlich diese zeitsparende Auswahl; `worklfow.md` bleibt der vorhandene Link auf `workflow.md`.

Offline geprüft werden Rezept-/Assetkonsistenz, Workflowprüfungen, gezielte Regressionen und betroffene bestehende Regressionen sowie der Release-Build. Es gibt keine bestätigten Spieltests. Installation: `references/release-0.1.59.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.58.dll` nach `Mods/FusionMod.dll` kopieren. Neue Spielstände benötigen 0.1.59 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen; Kosten, Cooldown, Abbruch, Vorschau, Markierung, Wasser, Dach, Unterlagen und Kürbisse. Black Cherry auf zwei Explosionen, Trefferzählung gegen Überlebende, erhöhten Folgeschaden, vorzeitiges Zerquetschen, Entfernen und Krater prüfen. Frozen Cherry auf geeignete und ungeeignete Geschosse, 1/2/10 Punkte, Kartenerzeugung, Kartenaufnahme und Tod mit zehn Sekunden Gefrieren prüfen. Demise auf globale Wirkung, Immunitäten und genau zehn Sekunden Wellensperre prüfen. Sundae auf drei Produktionen, 15 Zusatzsonne ausschließlich für bereits gefrorene Zombies, Kälte bei den ersten beiden Produktionen und Gefrieren beim letzten Vorgang prüfen. Alle Pflanzen bei Pause, Schlaf/Kaffee, Handschuh, Bungee, Fahrzeugkontakt, Tod und Speichern/Laden mitten in Fähigkeiten und Geschossflug prüfen. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert.

## Änderung 0.1.58: Gloom-Fusionen und Gutsy-shroom

```text
Fume-shroom | Gloom-shroom | Gloom-shroom
Frost-shroom | Gloom-shroom | Frost Gloom-shroom
Gloom-shroom | Ice-shroom | Frost Gloom-shroom
Flame-shroom | Gloom-shroom | Flame Gloom-shroom
Gloom-shroom | Jalapeno | Flame Gloom-shroom
Fume-shroom | Scaredy-shroom | Gutsy-shroom
```

Alle sechs Quellenrezepte für diese vier Ergebnisse sind umgesetzt. Sämtliche Zutaten waren vorher als native Pflanzen oder vollständig portierte Fusionen benutzbar. Gloom-shroom ist bereits nativ vorhanden; neu ist sein noch offenes Quellenrezept einschließlich Originaldarstellung und Quellenverhalten. Seine native Identität bleibt `Gloomshroom` (42), ohne Variantenmarker. Die zwei Elementarvarianten verwenden `Gloomshroom/Iceshroom` und `Gloomshroom/Jalapeno`; Gutsy-shroom verwendet `Fumeshroom/Scaredyshroom`. Beide Kartenreihenfolgen und Handschuhrichtungen nutzen die gemeinsame Rezeptauflösung, Terrainprüfung und Erzeugung vor Zutatenverbrauch. Kartenkosten und Cooldown stammen von der eingehenden Zutat. Seerosen, Töpfe und Kürbisse bleiben erhalten. Insgesamt sind 124 Rezepte aktiv; 544 Rezepte für 459 Ergebnisse bleiben offen. Die sechs erledigten Zeilen sind aus `fusionen.txt` entfernt. Folgefusionen und `fusionen-quellen.json` bleiben erhalten. Der vorhandene Workflow einschließlich des Links `worklfow.md` benötigt keine Änderung.

- **Gloom-shroom:** vier durchdringende Rauchpulse mit je 20 Schaden alle 1,9 Sekunden. Der Angriff erreicht geeignete Bodenziele vor, hinter und neben der Pflanze, höchstens eine Nachbarreihe entfernt. Die Quellenphysik verwendet einen Radius von zwei Quelleinheiten, etwa 1,48 Spalten; der Port prüft die Überlappung mit nativen Zielrechtecken. Die Originalereignisse nach 0,6 / 0,8667 / 1,1333 / 1,4 Clipsekunden werden bei Geschwindigkeit 1,4 ausgeführt.
- **Frost Gloom-shroom:** dieselben vier Pulse mit je 40 Schaden, Kälte und jeweils fünf Gefrierpunkten über den vorhandenen gemeinsamen Gefrierhelfer. Die Originalgeschwindigkeit beträgt 1,3. Native Kälteimmunitäten und das bisherige Gefriersystem bleiben zuständig.
- **Flame Gloom-shroom:** vier Pulse mit je 40 Grundschaden und Überhitzung über das vorhandene gemeinsame System. Bereits überhitzte Ziele erhalten dessen zusätzliche 50 Prozent Schaden. Kälteentfernung, Todesexplosion und Weitergabe der Überhitzung sind enthalten. Die originale zusätzliche Feuerebene bleibt ständig aktiv; Angriffsgeschwindigkeit ist 1,3.
- **Gutsy-shroom:** wählt beim Angriff das nächste geeignete Ziel vor sich in der eigenen Reihe. Unter sieben Quelleinheiten Abstand verwendet er durchdringenden Rauch mit 30 Schaden. Andernfalls erzeugt er zwei gestaffelte Originalsporen mit je 30 Direktschaden. Der Takt beträgt 1,5 Sekunden. Die Rauchanimation trifft nach 0,75 Sekunden; die doppelt schnelle Sporenanimation erzeugt Geschosse nach 0,375 und 0,5417 Sekunden. Die Pflanze versteckt sich auch bei nahen Zombies nicht. Sporen fliegen mit sechs Quelleinheiten pro Sekunde über die Reihe, verwenden native Bewegung und Kollision und werden durch Torchwood nicht entzündet.

Alle vier Ergebnisse besitzen 300 HP und behalten native Pilz-, Kaffee-, Terrain- und Handschuhregeln. Angriffsphase und Cooldown werden in `mStartRow` und `mLaunchCounter` gespeichert. Gutsy-shroom speichert zusätzlich den beim Angriff ausgewählten Modus in `mTargetX`; ein Laden zwischen den beiden Schüssen erzeugt ausschließlich den verbleibenden Schuss. Originalanimationen erzeugen keine zusätzlichen Treffer. Pause, Schlaf, Tragen, Bungee, Tod und Board-Ende stoppen die Fähigkeiten. Native `Fire`-Aufrufe sind für diese Identitäten unterdrückt. Gutsy-Sporen reservieren den Wert `-40030` in `Projectile.mCobTargetX` ausschließlich für native Erbsentypen. Native Serialisierung erhält Kennung und Flugbewegung; Initialisierung und Pool-Reset löschen die Kennung. Damit bleiben Schaden, Torchwood-Sperre und Originalgrafik auch für bereits fliegende Sporen nach Laden erhalten. Die vorhandenen flüchtigen Gefrierpunkte und Überhitzungszustände auf Zombies behalten ihre bisherigen Speichergrenzen.

Originalgrafik und sämtliche zehn Pflanzenclips stammen aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: GloomShroomPrefab 33647 / Transform 108374 / Controller 6300; IceGloomPrefab 33651 / 108378 / 6301; FireGloomPrefab 33650 / 108377 / 6299; ScaredFumePrefab 48132 / 92173 / 6628. Die Originalspore Bullet_puff 73653 / Transform 95616 hat keinen Animator. Der Export erhält alle Ebenen, Geschwindigkeiten, Referenzposen und Bindungen. Die alternative Sporen- und Rauchanimation von Gutsy-shroom bleibt jeweils eine eigene additive Kopfbewegung. Kein unbekannter Bindungspfad wird ignoriert. Die drei Gloom-Pflanzen erzeugen keine fliegenden Kollisionsprojektile. Rauchpartikel, Trefferpartikel, Kältepartikel und Geräusche verwenden vorhandene native Effekte. Unity-Spielskripte werden nicht ausgeführt.

Quellenrezepte, chinesische Almanachbeschreibungen, tatsächlich disassemblierte Methoden, Prefab-/Scriptidentitäten, Originalcontroller und Assetprüfsummen liegen unter `references/gloom-shroom/`, `references/frost-gloom-shroom/`, `references/flame-gloom-shroom/` und `references/gutsy-shroom/`. Die Geschossbelege liegen unter `references/gutsy-projectile/` und `references/gutsy-bullet-base/`; der gemeinsame Verhaltens- und Speicherbeleg steht in `references/gloom-shroom/port-notes.md`. Offline geprüft sind Rezept- und Assetkonsistenz, 13 Workflowprüfungen, alle 14 vorhandenen Regressionstestprogramme, die Originalanimationen und der Release-Build. Es gibt keine bestätigten Spieltests. Installation und Prüfsummen: `references/release-0.1.58.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.57.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit den neuen Elementar- oder Gutsy-Varianten benötigen 0.1.58 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle sechs Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten, Cooldown, Abbruch, Hervorhebung, Vorschau, Wasser, Dach, Unterlagen und Kürbisse. Gloom-Pulse auf vier Treffer, 1,9-Sekunden-Takt, Ziele hinten/vorne/seitlich und Radiusgrenze prüfen. Frost auf Kälte, jeweils fünf Gefrierpunkte und Immunitäten prüfen. Flame auf Überhitzung, erhöhten Folgeschaden und Todesfolgen prüfen. Gutsy auf Rauch bei nahen Zielen, zwei Fernsporen, Moduswechsel zwischen Salven, Ziele hinter der Pflanze, fehlenden Angstzustand und Torchwood-Sperre prüfen. Bei allen Varianten Originalanimationen, Pause, Schlaf/Kaffee, Handschuh, Bungee, Tod und Speichern/Laden während Angriff und Projektilflug prüfen. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert.

## Änderung 0.1.57: vier Threepeater-Fusionen

```text
Garlic | Threepeater | Stench-spreader
Ice-shroom | Threepeater | Snow Threepeater
Jalapeno | Threepeater | Scorched Threepeater
Jalapeno | Scorched Threepeater | Charred Threepeater
```

Das lokale Quellenarchiv enthält genau diese vier Rezepte für die vier neuen Ergebnisse. Die ersten drei Rezepte verwenden bereits benutzbare native Zutaten. Anschließend wird der vollständig implementierte Scorched Threepeater als Zutat für Charred Threepeater verfügbar. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden dieselbe Rezeptauflösung, Terrainprüfung und Erzeugung vor dem Zutatenverbrauch. Kosten und Cooldown stammen weiterhin von der eingehenden Karte. Folgefusionen, insbesondere Phoenix Threepeater, bleiben offen.

- **Stench-spreader:** drei Knoblaucherbsen mit je 30 Schaden alle 1,5 Sekunden in die eigene und benachbarte Reihe. Am Rand ersetzt ein zusätzlicher Schuss in der eigenen Reihe nach 0,2 Sekunden den fehlenden Seitenschuss. Alle Geschosse verwenden die vorhandenen Giftstufen, Vergiftung und Giftwolken. `Miasma.Update` zählt diese Pflanze mit drei Schüssen für die Wolkenschadensberechnung in ihrer Standreihe; Seitenschüsse erhöhen nicht die Pflanzenzahl fremder Reihen.
- **Snow Threepeater:** drei Eisgeschosse pro Sekunde. Er wählt bis zu drei verschiedene Reihen nach dem jeweils am weitesten links stehenden erreichbaren Zombie. Freie Schussplätze ergänzt er ohne Dopplung mit eigener, vorheriger und nächster Reihe. Die native Threepeater-Flugkurve wird für entfernte Zielreihen skaliert. Die vorhandene Schneeprojektil-Implementierung übernimmt 20 Schaden, Kälte, zehn Gefrierpunkte und vierfachen Schaden gegen gefrorene Ziele.
- **Scorched Threepeater:** drei gewöhnliche Erbsen mit je 40 Schaden alle 1,5 Sekunden; Randersatz wie beim Stench-spreader. Die erfolgreiche Fusion löst in eigener und benachbarter Reihe jeweils den belegten Jalapeno-Effekt mit 1800 Ascheschaden, Entfernung der Kälte und der bestehenden Heilung des Flame Tall-nut aus. Laden oder bloßes Erzeugen der gespeicherten Variante wiederholt diesen Effekt nicht. Gewöhnliches Torchwood bleibt zulässig: `BoardAction.FirePeas` addiert 20 Schaden, also 60 Direktschaden nach Entzündung; der native Flächenschaden wird entsprechend angepasst. Dünger verwandelt nur diese Variante nach Abschluss des gespeicherten Fütterungsvorgangs zurück in einen gewöhnlichen Threepeater.
- **Charred Threepeater:** die vollständig verbrannte Variante besitzt 300 HP und die originale Ruheanimation, aber keinen Angriff. Native Schussereignisse bleiben ebenfalls unterdrückt. Eine weitere Jalapeno-Fusion zu Phoenix Threepeater wird erst mit dessen vollständiger Implementierung verfügbar.

Alle vier Varianten behalten 300 HP, native Threepeater-Terrainregeln, Unterlagen, Kürbisse, Handschuhfähigkeit und gespeicherte Variantenidentität. Die zentral reservierten Speicherpaare sind `Threepeater/Garlic`, `Threepeater/Iceshroom`, `Threepeater/Jalapeno` und `Threepeater/Doomshroom`. `mStartRow` speichert die Angriffsphase und `mLaunchCounter` den Cooldown. Der originale mittlere Schussclip löst den Angriff bei 0,5 Sekunden aus. Native Animationsevents erzeugen keine zweiten Geschosse. Pause, Tragen, Bungee, Schlaf und Tod stoppen die Simulation über den bestehenden Aktivitätsweg. Der besondere Erbsenschaden wird als eindeutig negativer Wert `-40040` im bei Erbsen unbenutzten nativen `Projectile.mCobTargetX` gespeichert. Native Initialisierung und Pool-Reset löschen das Feld; Serialize/Deserialize erhalten es, Torchwood verändert es nicht. So bleibt der Schaden bereits fliegender Scorched-Geschosse beim Laden erhalten. Vorhandene Giftstufen, Giftwolken und Gefrierpunkte bleiben wie bisher flüchtig; deren Grenzen werden durch diese Erweiterung nicht geändert.

Originalgrafik und sämtliche 22 Clips stammen aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: GarlicThreePeaterPrefab 63448 / Transform 110668 / Controller 6512; IceThreePeaterPrefab 63450 / 110670 / 6512; DarkThreePeaterPrefab 63449 / 110669 / 6512; AshThreePeaterPrefab 63452 / 110672 / 6506. Die drei aktiven Pflanzen besitzen eine Körperebene und drei unabhängig additive Kopfebenen, jeweils mit alternativen Ruhe- und Schussclips. Der Importer erhält alle Ebenen, Referenzposen, Spritehierarchien und Sichtbarkeitsbindungen. Charred Threepeater nutzt ausschließlich seinen originalen Ruheclip. Stench-Geschosse verwenden das bereits exportierte Originalprojektil 34023; Scorched-Geschosse das bereits exportierte gewöhnliche Original-Erbsenmodell 33962. Schnee- und entzündete Feuererbsen verwenden die vorhandenen nativen Replanted-Controller und Effekte. Jalapeno-, Kälte-, Giftpartikel und Geräusche verwenden die vorhandenen nativen Effekte. Es werden keine Unity-Spielskripte ausgeführt.

Quellen, Almanachbeschreibungen, Rezeptnachweise, vollständige Gameplay-Prefabs, Scriptidentitäten, Animationsdaten, Prüfsummen und Offlinevorschauen liegen unter `references/stench-spreader/`, `snow-threepeater/`, `scorched-threepeater/` und `charred-threepeater/`. Die Belege für Giftwolken liegen unter `references/stench-spreader-cloud/`, für Fusions- und Düngerereignisse unter `references/scorched-threepeater-effects/`. Der gemeinsame Verhaltens- und Speicherbeleg steht in `references/stench-spreader/port-notes.md`. Offline geprüft werden die Rezept- und Assetkonsistenz, Workflowprüfungen, gezielte Threepeater-Regressionen und betroffene vorhandene Gift-, Feuer-, Nuss- und Squash-Regressionen sowie der Release-Build. Das Spiel wird nicht gestartet; Profile und Spielstände werden nicht verändert. Die Installation ist unter `references/release-0.1.57.json` dokumentiert. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.56.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit den neuen Varianten benötigen 0.1.57 oder eine kompatible neuere Version.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen; Kosten, Cooldown, Abbruch, Vorschau und Hervorhebung; Wasser, Dach, Unterlagen und Kürbisse. Stench-spreader auf Giftstufen, Vergiftung, Wolken und deren Schaden in eigener und fremder Reihe prüfen. Snow Threepeater auf Priorisierung mehrerer Zombies je Reihe, entfernte Zielreihen, fünf und sechs Reihen, freie Schussplätze und Gefriereffekte prüfen. Scorched Threepeater auf die drei einmaligen Feuerreihen, 40 Erbsenschaden, 60 Feuererbsenschaden samt Flächenschaden und Dünger-Rückwandlung prüfen. Charred Threepeater darf auch nach Laden oder Tragen nicht schießen. Bei allen Varianten Randersatzschüsse, Pause, Bungee, Tod und Speichern/Laden während Angriff und Projektilflug prüfen.

## Änderung 0.1.56: Icerock, Hearthrock, Foul-shroom und Starch-shroom

```text
Spikerock | Ice-shroom | Icerock
Iceweed | Spikerock | Icerock
Spikerock | Jalapeno | Hearthrock
Scorchweed | Spikerock | Hearthrock
Fume-shroom | Garlic | Foul-shroom
Fume-shroom | Potato Mine | Starch-shroom
```

Das vollständige lokale Quellenarchiv enthält genau diese sechs Rezepte für die vier neuen Ergebnisse. Alle Zutaten waren bereits als native Replanted-Pflanzen oder vollständige Fusionen vorhanden. Native Spikerock (SeedType 46) ist jetzt zusätzlich in der geprüften Zutatenliste belegt; sein eigenes noch offenes Fusionsrezept bleibt unverändert. Beide Kartenreihenfolgen und Handschuhrichtungen verwenden die gemeinsame Rezeptauflösung, Platzierungsprüfung und Erzeugung vor Zutatenverbrauch. Native Terrainregeln, Kosten, Cooldown, Vorschau, Hervorhebung, Seerosen, Töpfe und Kürbisse bleiben erhalten. Die sechs erledigten Zeilen sind aus `fusionen.txt` entfernt; Folgefusionen bleiben offen. Jetzt sind 114 Rezepte aktiv; 554 Rezepte für 467 Ergebnisse bleiben offen. `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert; eine Workflowänderung ist nicht notwendig.

Icerock und Hearthrock behalten native Spikerock-KI, 450 HP, niedrige Zielkategorie, Fahrzeugkontakt, Hammerreaktionen und Speicherung. Sie verursachen pro Sekunde zwei Treffer mit je 40 Grundschaden. Native Fahrzeugkontakte verursachen weiterhin 1800 Schaden und nutzen den vorhandenen Abnutzungsweg. Icerock setzt nach einem Treffer 15 Sekunden Kälte und fünf Gefrierpunkte über den vorhandenen gemeinsamen Gefrierhelfer. Native Immunitäten gelten. Hearthrock setzt nach einem Treffer den vorhandenen Überhitzungszustand einschließlich Schadensverstärkung, Todesexplosion und Weitergabe. Bereits überhitzte Ziele erhalten dessen vorhandene Schadensverstärkung. Die Originalgrafik verliert bei höchstens 300 beziehungsweise 150 HP die ersten beiden großen Spitzen. Leerlauf und beide Angriffsbewegungen stammen aus dem Originalcontroller. Die beiden Originalereignisse werden an die nativen Kontaktphasen angepasst und erzeugen keinen zusätzlichen Schaden.

Foul-shroom und Starch-shroom behalten 300 HP, native Pilz-/Coffee-Bean-Regeln und 1,5 Sekunden Angriffstakt. Der Treffer erfolgt beim Originalereignis nach 350 ms und durchdringt geeignete Bodenziele in der eigenen Reihe bis sieben Quelleinheiten nach vorne, etwa 414,815 Replanted-Pixel. Foul-shroom verursacht 30 Schaden und danach eine Stufe im vorhandenen gemeinsamen Giftzustand. Ab der sechsten Stufe beginnt dessen Giftschaden; ein gewöhnlicher Rauchangriff erzeugt keine Miasmawolke. Starch-shroom verursacht gewöhnlich ebenfalls 30 Schaden. Seine Aufladung beginnt mit 15 Sekunden. Die erste danach abgefeuerte Welle ersetzt den normalen Schaden für jedes getroffene Ziel durch eine 300-Schaden-Kartoffelexplosion. Der tatsächliche Aufruf von `AoeDamage.SmallBombPotato` hat Radius 0 und Reihenabstand 0: Der Port verwendet den vorhandenen Explosionshelfer als Punktüberlappung im nativen Zielrechteck. Mehrere sich überlappende Zielrechtecke können mehrere Explosionen erhalten. Es entsteht kein zusätzlicher gewöhnlicher 30-Schaden-Treffer. Jede geladene Welle verbraucht die Aufladung auch dann, wenn beim Trefferereignis kein Ziel mehr vorhanden ist. Danach beginnt die Aufladung erneut bei 15 Sekunden.

Die gespeicherten Identitäten sind `Spikerock/Iceshroom`, `Spikerock/Jalapeno`, `Fumeshroom/Garlic` und `Fumeshroom/Potatomine`. Angriffsphase und Takt der Pilze liegen wie bisher in `mStartRow` und `mLaunchCounter`. Starch-shroom verwendet die für native Fume-shroom außerhalb des unterdrückten Fire-Wegs unbenutzten Felder `mTargetX`, `mTargetY` und `mShootingCounter` für Aufladung, Blinkabstand und Blinkphase. Initialisierung, Reset, native Feldschreiber sowie Serialisierung/Laden sind geprüft. Das Originalblinken verwendet den Abstand max(0,5 Sekunden, Restaufladung / 5). Der additive Original-Layer besitzt Gewicht 0,97 und ausschließlich diskrete Spritebindungen. Aufladung und Blinkphase bleiben beim Speichern erhalten. Pause, Schlaf, Tragen, Bungee, Tod und Board-Ende stoppen die Fähigkeiten über den bestehenden Aktivitätsweg. Die bereits vorhandenen Gift- und Gefrierzustände auf Zombies bleiben flüchtig und werden beim Laden zurückgesetzt.

Keine dieser vier Pflanzen erzeugt eigene fliegende Kollisionsprojektile. Kälte, Gefrierpunkte, Überhitzung, Gift, durchdringender Rauch und sämtliche Kartoffelexplosionen sind umgesetzt. Originalgrafik und alle neun Clips stammen ausschließlich aus dem lokalen Originalbundle `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: IceSpikerockPrefab 60659 / Transform 89928 / Controller 6220; FireSpikerockPrefab 60661 / 89930 / 6220; GarlicfumePrefab 70211 / 100382 / 6276; PotatoFumePrefab 70226 / 100397 / 6284. Es gibt keine verworfenen unbekannten Bindungen. Bei den Felsvarianten ist das Original-Shadow-Objekt leer; der Originalkörper dient als Skalierungsfußabdruck. Rauchpartikel, Explosionspartikel, Kältepartikel und Geräusche verwenden native Replanted-Effekte. Unity-Spielskripte und Animationsevents werden nicht ausgeführt.

Quellen, Almanachbeschreibungen, Rezeptvollständigkeit, Prefab-/Scriptidentitäten, Assetprüfsummen und Offlinevorschauen liegen unter `references/icerock/`, `references/hearthrock/`, `references/foul-shroom/` und `references/starch-shroom/`. Der gemeinsame Verhaltens- und Speicherbeleg steht in `references/icerock/port-notes.md`; native Wege sind unter `references/weed-fusions/` und `references/fume-native/` belegt. Offline-Nachweise: Rezept-/Assetprüfung, 13 Workflowprüfungen, betroffene Fume-, Weed-, Poison-, Mine- und Elemental-Nut-Regressionen, Originalassetvorschauen und Release-Build. Die übrigen vorhandenen Regressionen prüfen die erweiterte gemeinsame Identitäts- und Rezeptregistrierung. Kein Spielstart, keine Spieltests, keine Änderungen an Profilen oder Spielständen. Installation und Versionsnachweis: `references/release-0.1.56.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.55.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen Version 0.1.56 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle sechs Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten, Cooldown, Abbruch, Terrain, Unterlagen und Kürbisse, Hervorhebung und Vorschau. Icerock auf beide Kontakte, 15 Sekunden Kälte, Gefrierpunkte, Fahrzeuge, Riesen und Grafik bei 300/150 HP prüfen. Hearthrock auf beide Kontakte, Überhitzung und Todesfolgen prüfen. Foul-shroom auf durchdringenden 30-Schaden-Rauch, Reichweite, Giftstufen und gemeinsame Wirkung mit Poison-Shootern prüfen. Starch-shroom auf erste 15 Sekunden, normale und geladene Wellen, überlappende Ziele, leere Wellen, Aufladung und Originalblinken prüfen. Bei allen Varianten Pause, Handschuhbewegung, Bungee, Tod, Pool-Wiederverwendung und Speichern/Laden während Angriff, Aufladung und Blinkphase prüfen.

## Änderung 0.1.55: Doom Squash, Jawbreaker, Doom Chomper und Frostbiter

```text
Chomper | Doom-shroom | Doom Chomper
Chomper | Ice-shroom | Frostbiter
Chomper | Squash | Jawbreaker
Doom-shroom | Squash | Doom Squash
```

Das vollständige lokale Quellenarchiv enthält genau ein Rezept je Ergebnis. Alle Zutaten waren bereits verfügbar. Alle vier Rezepte verwenden den gemeinsamen Ablauf für beide Kartenreihenfolgen und Handschuhrichtungen: native Geländeprüfung, Vorschau, Hervorhebung, Kosten und Cooldown, Erzeugung vor Zutatenverbrauch sowie Erhalt von Seerosen, Töpfen und Kürbissen bei der Fusion. Sofortzutaten werden nicht verzögert. Genau vier erledigte Zeilen wurden aus `fusionen.txt` entfernt; Folgefusionen bleiben offen. Jetzt sind 108 Rezepte aktiv; 560 Rezepte für 471 Ergebnisse bleiben offen. `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert. Eine notwendige Workflowänderung mit zusätzlicher Zeitersparnis ergab sich nicht.

Doom Squash und Jawbreaker verwenden native Squash-Zustände, Landung, 1800 Quetschschaden, Bewegung und Speicherung. Doom Squash sucht in der eigenen Reihe. Die tatsächliche Quelle `DoomSquash.get_Range` liefert eine Box von acht Quelleinheiten, deren Mittelpunkt die Basismethode zwei Quelleinheiten nach vorne versetzt. Mit der belegten Rasterbreite 1,35 ergeben sich etwa 118,519 Pixel nach hinten und 355,556 Pixel nach vorne; der Almanach verkürzt dies auf zwei beziehungsweise drei Felder. Bei der Landung folgt eine Doom-Explosion mit 1800 Ascheschaden und fünf Quelleinheiten Radius (etwa 296,296 Pixel). Die Explosion zerstört den Pflanzenstapel auf dem tatsächlichen Landefeld und erzeugt dort einen nativ gespeicherten Krater für 180 Sekunden. Ein vorhandener Krater wird aufgefrischt. Benachbarte Pflanzen werden durch diesen Krater nicht entfernt. Schaden und native Explosionsdarstellung verwenden den gemeinsamen Doom-Kelp-Schadensweg. Der zuvor mit 400 Pixeln zu groß angesetzte Doom-Kelp-Radius wird dabei auf dieselbe belegte Umrechnung mit 1,35 Quelleinheiten pro Spalte korrigiert. Doom Kelp erzeugt weiterhin keinen Krater.

Jawbreaker sucht ausschließlich weiter rechts liegende Zombie-Spalten in der eigenen Reihe. Der tatsächliche Quellbereich ist vier Quelleinheiten breit und zwei Einheiten nach vorne versetzt, also bis etwa 237,037 Pixel nach vorne. Der Almanach bezeichnet dies als die rechte Nachbarzelle. Bei der Landung verschlingt Jawbreaker höchstens einen geeigneten Zombie anstelle des Quetschtreffers. Die übrigen Ziele erhalten den gewöhnlichen Quetschtreffer. Riesen, Boss und Zielattrappen werden nicht verschlungen. Gewöhnliche Fahrzeuge bleiben wie im nativen Chomper und in der Quellregel für Nicht-Riesen verschlingbar. Erfolgreiches Verschlingen setzt einen gespeicherten Rückkehrmarker. Beim anschließenden Tod kehrt Jawbreaker kostenlos am aktuellen Landefeld zurück, sofern dessen native Geländeprüfung erfolgreich ist. Wie `ChomperSquash.DieEvent` schließt der Port die letzte Spalte aus. Schaufel, Zutatenverbrauch, Bungee, Laden und Board-Ende lösen keine Rückkehr aus. Ohne erfolgreiches Verschlingen gibt es keine Rückkehr. Der sonst unbenutzte native Squash-`mLaunchCounter` speichert den Marker; `mStartRow` bleibt für die Sprungbahn reserviert.

Doom Chomper übernimmt native Chomper-Zielfilter, Verschlingen und Fehlbisse. Der tatsächliche Quellcast beginnt eine Quelleinheit vor dem Schatten und reicht zwei weitere Quelleinheiten nach vorne. Diese Position und die Reichweite von etwa 118,519 Pixeln werden in den vom nativen Zielsucher aufgerufenen Angriffsbereich übertragen. Nur der erfolgreiche Übergang `ChomperBiting` nach `ChomperBitingGotOne` erzeugt einmal eine Doom-Explosion eine Spalte vor der Pflanze. Schaden, Radius, Verlust des dortigen Pflanzenstapels und Krater entsprechen Doom Squash. In der letzten Spalte gibt es wie in der Quelle kein Explosionsfeld außerhalb des Boards. Die Verdauung dauert 60 Sekunden statt der nativen 40 Sekunden und liegt im gespeicherten nativen Countdown. Ein Fehlbiss, ein nicht verschlungenes großes Ziel oder Laden löst keine Explosion aus.

Frostbiter verwendet denselben Quellcast-Ursprung und die um eine Quelleinheit vergrößerte Reichweite aus `IceChomper.get_ChompRange`: drei Quelleinheiten beziehungsweise etwa 177,778 Pixel. Native Zielfilter bleiben erhalten. Ungefrorene Ziele können einen Angriff auslösen, werden am tatsächlichen Bissereignis aber nicht verschlungen. Erfolgreiches Verschlingen eines gefrorenen geeigneten Ziels erzeugt einen boardweiten Ice-shroom-Effekt mit 20 Schaden und einer Sekunde Gefrieren; längeres vorhandenes Gefrieren bleibt erhalten. Die native Gefrierfunktion bewahrt Immunitäten und Darstellung. Nach jedem Biss erzeugt die tatsächliche Methode `IceChomper.BiteEvent` zusätzlich einen lokalen Puls, auch nach erfolgreichem Verschlingen: 40 Schaden, 15 Sekunden Kälte und 51 Punkte im vorhandenen gemeinsamen Gefrierzustand, Radius zwei Quelleinheiten (etwa 118,519 Pixel) und höchstens eine Nachbarreihe. Dies erweitert die verkürzte Almanachbeschreibung, die den Puls nur beim Fehlschlag erwähnt. Der Almanach nennt 50 Punkte; der tatsächliche Aufruf von `Zombie.AddfreezeLevel` erhält `0x33`, also 51, und addiert diesen Wert unverändert. Der native einzelne 40-Schaden-Biss gegen Riesen wird dabei ersetzt, damit kein doppelter Treffer entsteht. Verdauung bleibt 40 Sekunden. Bereits gefrorene oder immune Ziele erhalten über den bestehenden Gefrierhelfer keine zusätzlichen Punkte. Der einsekündige globale Effekt heilt vorhandene Frost Tall-nuts über den bestehenden Helfer.

Alle vier Varianten haben 300 HP. Zentral reservierte Speicherpaare sind `Squash/Doomshroom`, `Squash/Chomper`, `Chomper/Doomshroom` und `Chomper/Iceshroom`. Native Typen bleiben für Gelände, KI, Handschuh und Speicherung erhalten; Marker unterscheiden Varianten. Synchrone Kontexte werden mit Finalizern beziehungsweise `finally` wiederhergestellt. Ziele werden vor Flächenschaden mit Board und `DataID` erfasst. Die vorhandenen Gefrierpunkte bleiben wie bisher flüchtig und werden beim Laden zurückgesetzt; Pflanzenidentität, Verdauung, Sprung und Rückkehrmarker werden nativ gespeichert. Quellen-Upgrades und nicht implementierte Folgefusionen sind keine zusätzlichen Grundfähigkeiten.

Diese vier Pflanzen erzeugen keine eigenen fliegenden Kollisionsprojektile. Verschlingen, Quetschen, Doom-Explosionen, Gefrieren und der Kältepuls sind umgesetzt. Originalgrafik, Hierarchien, Pivots, Schatten und sämtliche 18 Controllerclips stammen ausschließlich aus dem lokalen Bundle `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: DoomSquashPrefab 68176 / Transform 91104 / Controller 6477; ChomperSquashPrefab 68169 / 91097 / 6477; DoomChomperPrefab 34840 / 93310 / 6239; IceChomperPrefab 34843 / 93313 / 6239. Zwei Blickclips von Jawbreaker und der Doom-Chomper-Controller enthalten nachweislich Kurven für fehlende Transformpfade; genau diese vom Original selbst ignorierten Bindungen sind dokumentiert. Die übrigen Kurven bleiben vollständig erhalten. Unity-Spielskripte und Animationsevents werden nicht ausgeführt. Native Weltbewegung bleibt der einzige Bewegungsanker. Explosionen, Eispartikel, Kratergrafik und Geräusche verwenden native Replanted-Effekte.

Quellenbelege, Almanachtexte, Rezeptvollständigkeit, Prefab-/Scriptidentität und Asset-Prüfsummen stehen in den vier Ergebnisverzeichnissen unter `references/`, ergänzend in `doom-crater/`, `chomper-squash-native/`, `chomper-squash-animation-bindings.json` und den bestehenden `squash-kelp-effects/`-Belegen. Offline-Nachweise: Release-Build, Rezept-/Assetprüfung, Workflowprüfung, isolierte Squash-/Chomper-Regressionen sowie betroffene vorhandene Regressionen. Kein Spielstart, keine Spieltests, keine Änderungen an Profilen oder Spielständen. Installation und Versionsnachweis: `references/release-0.1.55.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.54.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen Version 0.1.55 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten, Cooldown, Terrain, Unterlagen, Kürbisse, Vorschau, Hervorhebung und Abbruch. Doom Squash auf Zielbereich, Landeposition, beide Schadensereignisse, Pflanzenverlust am Kraterfeld und Kraterdauer prüfen. Jawbreaker auf einmaliges Verschlingen, große Ziele, Quetschen weiterer Ziele, Rückkehr am Landefeld, besetzte Landefelder, letzte Spalte und Ausschluss von Schaufel/Zutatenverbrauch prüfen. Doom Chomper auf erfolgreiche und erfolglose Bisse, Explosion eine Spalte voraus und 60 Sekunden Verdauung prüfen. Frostbiter auf größere Reichweite, gefrorene/ungefrorene/immune Ziele, 40-Schaden-Puls, tatsächlich 51 Gefrierpunkte, Kälte, globale 20 Schaden und eine Sekunde Gefrieren sowie große Ziele ohne doppelten Biss prüfen. Originalgrafik und alle Animationen, Pause, Handschuh, Bungee, Tod, Pool-Wiederverwendung und Speichern/Laden während Sprung, Verdauung und nach Verschlingen prüfen.

## Änderung 0.1.54: Frost-shroom, Flame-shroom, Soot-shroom und Fume-spreader

```text
Fume-shroom | Ice-shroom | Frost-shroom
Fume-shroom | Jalapeno | Flame-shroom
Doom-shroom | Fume-shroom | Soot-shroom
Fume-shroom | Threepeater | Fume-spreader
```

Das vollständige lokale Quellenarchiv enthält genau ein Rezept für jedes dieser vier Ergebnisse. Alle Zutaten waren bereits verfügbar. Die gemeinsame Rezeptauflösung unterstützt beide Kartenreihenfolgen und Handschuhrichtungen. Platzierungsprüfung, Kosten, Cooldown, Vorschau, Hervorhebung, Erzeugung vor Zutatenverbrauch sowie der Erhalt von Seerosen, Töpfen und Kürbissen bleiben im vorhandenen Ablauf. Die vier vollständig erledigten Zeilen sind aus `fusionen.txt` entfernt. Jetzt sind 104 Rezepte aktiv; 564 Rezepte für 475 Ergebnisse bleiben offen. `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert; eine Workflowänderung bringt hier keine notwendige Zeitersparnis.

Alle vier Varianten besitzen 300 HP und verwenden die native Fume-shroom-Identität für Gelände, Schlaf, Coffee Bean, Bungee, Handschuh und Speicherung. Die Paare `Fumeshroom/Iceshroom`, `Fumeshroom/Jalapeno`, `Fumeshroom/Doomshroom` und `Fumeshroom/Threepeater` sind zentral reserviert. `mLaunchCounter` speichert den Angriffstakt beziehungsweise das Nachladen. Das bei Fume-shroom unbenutzte `mStartRow` speichert die laufende Originalanimation. Initialisierung, Reset, Speicherung und Laden sind anhand der nativen Methoden geprüft. Die Varianten ersetzen nur `UpdateShooter`; die gemeinsamen Schlaf-, Aufwach- und Bungee-Prüfungen in `UpdateAbilities` bleiben wirksam. Native `Fire`-Callbacks erzeugen keinen doppelten Treffer. Pause, Schlaf und Tragen stoppen den Angriff.

Frost-shroom verursacht 40 durchdringenden Schaden alle 1,5 Sekunden. Jeder Treffer verursacht native Kälte und fünf Gefrierpunkte im bestehenden gemeinsamen Gefrierzustand. Die Schwelle beginnt bei 100 Punkten, steigt nach ausgelöstem Gefrieren um 100 und endet bei 1000. Native Immunitäten gelten; bereits gefrorene Ziele erhalten keine zusätzlichen Punkte. Es entstehen weder zusätzlicher Ice-shroom-Schaden noch Schneegeschoss-Schadensmultiplikatoren. Frost-shroom und Flame-shroom verwenden den tatsächlichen Quellbereich von `shadow.x` bis `shadow.x + 7`, Die tatsächliche Rasterfunktion `Mouse.GetColumnFromX` teilt durch 1,35 Quelleinheiten pro Spalte. Bei 80 Pixeln pro Replanted-Spalte ergibt das etwa 414,815 Pixel nach vorne. Der Beleg liegt unter `references/fume-grid/`. Tote Zombies, verbündete Zombies, fliegende Ziele, Grabsteine und Zielattrappen sind ausgeschlossen; die native Fume-shroom-Schadenskategorie entscheidet über weitere Zielzustände.

Flame-shroom erzeugt pro Angriff 33 einzelne Feuertreffer mit je einem Grundschaden. Die Folge beginnt beim Originalereignis nach 350 ms und läuft mit 50 Hz bis zum Ende des einsekündigen Angriffsclips. Jeder Treffer entfernt Kälte und setzt den vorhandenen Überhitzungszustand: nachfolgender Schaden erhöht sich um 50 Prozent; der bestehende Todespfad erzeugt dessen Explosion und Weitergabe. Ganzzahlige native Schadenswerte bleiben erhalten. Die Flamme ist kein zusätzliches fliegendes Projektil. Originalclips `idle`, `shoot` und die unabhängige Spriteanimation `fire` sind eingebettet.

Soot-shroom greift ausschließlich durch einen gewöhnlichen Linksklick an. Der Treffer erfolgt nach 350 ms und verursacht 1800 durchdringenden Ascheschaden an allen geeigneten Zombies vor der Pflanze in derselben Reihe. Es gibt keine obere Reichweitengrenze, keinen Krater und keinen Pflanzenschaden. Nach Auslösung lädt die Pflanze 30 Sekunden nach; `rest` zeigt diesen Zustand. Erneute Klicks während des Angriffs oder Nachladens erzeugen keinen weiteren Treffer. Schaufel, Handschuh und fremde Spieler lösen keinen Angriff aus. Der vollständige Originalclip dauert 1,55 Sekunden.

Fume-spreader trifft die eigene und beide benachbarten Reihen über deren gesamte Länge nach vorne. Jeder Treffer verursacht 40 durchdringenden Schaden. In der obersten oder untersten vorhandenen Reihe verursacht die eigene Reihe 80 Schaden, die benachbarte Reihe weiterhin 40. Der Originalcode `ThreeFume.DelayShoot` wartet nach dem 350-ms-Animationsereignis zusätzlich 200 ms; Schaden entsteht deshalb nach 550 ms. Der Angriffstakt beträgt 1,5 Sekunden. Der vollständige Originalclip dauert etwa 1,017 Sekunden und wird auf 102 Simulationsticks abgebildet. Die Randregel verwendet die tatsächliche Anzahl der Reihen und gilt auch im Pool.

Keine dieser vier Pflanzen erzeugt ein fliegendes Kollisionsprojektil. Rauch und Flammen verursachen direkte durchdringende Treffer; ihre Fähigkeiten und Statuswirkungen sind umgesetzt. Native FumeCloud-Partikel und Fume-Geräusche ersetzen die fremden Unity-Partikelsysteme. Die drei belegten Partikelemissionen im Fume-spreader-Clip werden ausdrücklich durch die drei nativen Rauchwolken ersetzt. Die originalen Pflanzensprites, Hierarchien, Pivots, Schatten sowie alle zehn Controllerclips stammen aus `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: IceFumeShroomPrefab 70222 / Transform 100393 / Controller 6610; FireFumePrefab 70230 / 100401 / 6274; DoomFumePrefab 70203 / 100374 / 6594; ThreeFumePrefab 70217 / 100388 / 6283. Der vorhandene Import-Renderer übernimmt sowohl additive als auch ersetzende Schussposen. Unity-Spielskripte und Animationsevents werden nicht ausgeführt.

Belege: Almanach, alle Ergebnisrezepte, tatsächliche Methoden, Prefab-/Scriptidentitäten und Exportprüfsummen unter `references/frost-shroom/`, `flame-shroom/`, `soot-shroom/` und `fume-spreader/`; ergänzend `fume-base/`, `fume-shooter/`, `fume-spreader-sequence/` und `fume-native/`. Die Gefrierpunkte und Überhitzung verwenden die bestehenden flüchtigen Zombie-Zustände und werden beim Laden wie bisher nicht wiederhergestellt. Pflanzenidentität, Angriffsphase und Nachladen werden nativ gespeichert. Globale Quellen-Upgrades sind keine zusätzlichen Grundfähigkeiten dieser Rezeptvarianten.

Offline-Nachweise: Release-Build, Rezept-/Assetprüfung, Workflowprüfungen, isolierte Fume-Regressionsprüfung sowie bestehende Gift-, Feuer- und Gefrierregressionen. Kein Spielstart, keine Spieltests, keine Änderungen an Profilen oder Spielständen. Installation und Versionsnachweis: `references/release-0.1.54.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.53.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit diesen Varianten benötigen Version 0.1.54 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten, Cooldown, Terrain, Unterstützungspflanzen und Kürbisse, Vorschau, Hervorhebung und Abbruch. Frost-shroom auf 40 Schaden, Kälte, fünf Gefrierpunkte und Immunitäten prüfen. Flame-shroom auf die vollständige Flammenfolge, Überhitzung, Kälteentfernung und Todesexplosion prüfen. Soot-shroom auf Linksklick, 1800 Schaden, gesamte Vorderreihe, 30 Sekunden Nachladen und Abgrenzung von Schaufel/Handschuh prüfen. Fume-spreader auf drei Reihen, Verzögerung, gesamte Vorderreihe sowie 80/40 Schaden an beiden Rändern auf fünf und sechs Reihen prüfen. Originalgrafik, Schlaf/Coffee Bean, Pause, Bungee, Tod, Pool-Wiederverwendung und Speichern/Laden während Angriff und Nachladen prüfen.

## Änderung 0.1.53: Starry Squash, Solar Squash, Spicy Kelp und Doom Kelp

```text
Squash | Starfruit | Starry Squash
Squash | Sunflower | Solar Squash
Tangle Kelp | Jalapeno | Spicy Kelp
Tangle Kelp | Doom-shroom | Doom Kelp
```

Das vollständige lokale Quellenarchiv enthält genau ein Rezept je Ergebnis. Alle benötigten Zutaten waren bereits verfügbar. Alle vier Rezepte unterstützen beide Kartenreihenfolgen und Handschuhrichtungen über die gemeinsame Rezeptauflösung. Die Ergebniskennung wird vor der Controller-Erzeugung gesetzt; Zutaten werden erst nach erfolgreicher Erzeugung verbraucht. Kartenkosten, Cooldown, Handschuhkosten, Vorschau und Hervorhebung bleiben im bestehenden gemeinsamen Ablauf. Unterlagen und Kürbisse bleiben erhalten. Kelp-Ergebnisse verwenden die native Wasserprüfung von Tangle Kelp; das Rezept umgeht keine Geländeprüfung. Sofortpflanzen werden nicht künstlich verzögert. Genau vier erledigte Zeilen sind aus `fusionen.txt` entfernt. Jetzt sind 100 Rezepte aktiv; 568 Rezepte für 479 Ergebnisse bleiben offen. `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert.

Starry Squash verwendet die native Squash-Zielsuche im belegten Bereich von einem halben Feld links bis einem Feld rechts sowie den gespeicherten nativen Sprungablauf. Der zentrale Quetschtreffer verursacht 900 statt 1800 Schaden. Beim gleichen Landungsereignis folgen vier Kreise mit je einem Quellfeld Radius und 900 Schaden an den belegten Offsets (+/-1,5; +/-0,5) Quellfelder. Die zusätzlichen Treffer betreffen höchstens die benachbarten Reihen; ein großes Ziel kann mehrere Kreise berühren. Flugziele und verbündete Zombies bleiben ausgeschlossen. Native Schilde und Schadensregeln bleiben erhalten. Der Port erzeugt keine zusätzliche Feuerwelle. Der Originalcontroller enthält die fünf Körper und alle fünf Leerlauf-, Blick-, Sprung- und Landungsclips. Native Weltbewegung und Originalpose werden nur einmal angewendet.

Solar Squash verursacht den gewöhnlichen Quetschschaden von 1800. Nach erfolgreichem nativem Tod versucht die Pflanze, in ihrer aktuellen Landespalte zurückzukehren. Die erste Wiederkehr kostet 50 Sonne, jede weitere 25 mehr. Die ursprüngliche Pflanzspalte wird nicht irrtümlich als Landeposition verwendet. Das Zielgelände wird erneut nativ geprüft. Eine fehlgeschlagene Erzeugung erstattet die reservierten Kosten; auch bei einer Ausnahme bleibt die Reservierung nicht verloren. Wenn keine Wiederkehr gelingt, entstehen zwei gewöhnliche Sonnenpickups mit zusammen 50 Sonne. Wie im tatsächlichen `SunSquash.DieEvent` gilt dieser letzte Zweig auch beim Schaufeln; Schaufeln löst keine Wiederkehr aus. Fusionsverbrauch, Tragen, Bungee, Laden, wiederholter Tod und Board-Ende lösen keine Wiederkehr oder Erstattung aus. Auch Tod durch Quetschen bleibt erfasst. `mLaunchCounter` speichert die Wiederkehrgeneration; `mStartRow` bleibt ausdrücklich für die native Sprungbahn reserviert. Alle sechs Originalclips sind eingebettet; die kontinuierliche additive Sonnenrotation läuft zusätzlich zur jeweiligen Squash-Pose.

Spicy Kelp und Doom Kelp behalten native aquatische Platzierung, Zielregeln, Greifen, Ertränken und Entfernung. Spicy Kelp verursacht bei Tod zusätzlich 1800 Ascheschaden in der gesamten eigenen Reihe, entfernt Kälte und heilt vorhandene Flammennüsse über den gemeinsamen Feuerhelfer. Doom Kelp verursacht zusätzlich 1800 Ascheschaden im Radius von fünf Quellfeldern. Dieser Radius stammt aus der tatsächlichen Methode `BoardAction.SetDoom`, nicht aus dem kleineren nativen Replanted-Doomradius. Es entstehen weder Krater noch Pflanzenschaden. Tote, verbündete und fremden Boards zugehörige Ziele bleiben ausgeschlossen. Beide Varianten verwenden stabile Ziel-Snapshots mit `DataID`, damit Schadenscallbacks keine wiederverwendeten Zombies treffen. Nur erfolgreich abgeschlossener Tod löst den Effekt aus; Fusionsverbrauch, Laden, Tragen und Board-Ende sind ausgeschlossen. Die gespeicherten nativen Typ-/Markerpaare rekonstruieren Verhalten und Grafik nach dem Laden. Es gibt keinen flüchtigen Fähigkeitenzustand.

Keine dieser vier Pflanzen verschießt eigene Flugprojektile. Die fünf Quetschbereiche, Wiederkehr und Kelp-Todesexplosionen sind vollständig als Fähigkeiten umgesetzt. Greifen und Ertränken bleiben native Fähigkeiten. Originalgrafik und sämtliche Controllerclips stammen ausschließlich aus `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: StarSquashPrefab 68167 / Transform 91095 / Controller 6478; SunSquashPrefab 68180 / Transform 91108 / Controller 6479; FirekelpPrefab 75434 / Transform 93164 / Controller 6503; DoomKelpPrefab 75433 / Transform 93163 / Controller 6503. FirekelpPrefab trägt tatsächlich das Basisscript `Tanglekelp`; der gesonderte Feuer-Todespfad ist in `Jalakelp.DieEvent` belegt. Diese unterschiedliche Prefab- und Verhaltensidentität ist dokumentiert. Unity-Spielskripte werden nicht ausgeführt.

Kelp verwendet seine originale Wasseroberfläche als Fußabdruck, weil sein Schattenobjekt kein Sprite enthält. Der zweite additive Controller-Layer steuert die Originalgreifarme; ihre unabhängige Gruppe folgt dem tatsächlich gespeicherten nativen Greifziel statt der zufälligen Szenenposition im Prefab. Der native Ertränkungsablauf übernimmt Wasser-/Zombieeffekte; das fremde SpriteMask-Spielobjekt wird nicht instanziiert. Explosionspartikel, Wasser-/Ertränkungseffekte und Geräusche verwenden native Replanted-Effekte. Das ist ein Effekt-Ersatz; die originalen Pflanzensprites, Hierarchien, Pivots und Kurven bleiben eingebettet. Belege und Assetprüfsummen liegen in den vier Ergebnisverzeichnissen sowie `references/squash-kelp-effects/`, `references/spicy-kelp-factory/` und `references/squash-kelp-native/`.

Offline-Nachweise: Rezept-/Assetprüfung, Workflowprüfungen, betroffene isolierte Regressionen, visuelle Prüfung der exportierten Originalassets und Release-Build. Die Recherche öffnet PE-Dateien jetzt mit `fast_load=True`; Metadaten und Codebereiche bleiben identisch, ohne unnötige PE-Verzeichnisanalyse. Dafür ist keine Workflowänderung erforderlich. Kein Spielstart, keine Spieltests, keine Änderungen an Profilen oder Spielständen. Installation und Versionsnachweis: `references/release-0.1.53.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.52.dll` nach `Mods/FusionMod.dll` kopieren. Neue Spielstände mit diesen Varianten benötigen Version 0.1.53 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Karten- und Handschuhrichtungen, Kosten, Cooldown, Terrain, Unterlagen, Kürbisse, Hervorhebung und Abbruch. Starry Squash auf fünf Trefferbereiche, große Ziele und alle Sprungphasen prüfen. Solar Squash auf Landeposition, Kostenfolge 50/75/100, fehlende Sonne, besetzte Zielfelder, Schaufel-Erstattung und Tod durch Quetschen prüfen. Beide Kelp-Varianten im Wasser auf Greifen, Ertränken, Tod durch andere Ursachen, Feuerreihe beziehungsweise Doomradius, fehlenden Krater und unbeschädigte Pflanzen prüfen. Originalgrafik, additive Animationen, Pause, Handschuhbewegung, Laden während eines Sprungs oder Griffs sowie Pool-Wiederverwendung prüfen.

## Änderung 0.1.52: Butter-nut, Umbrella Husk, Torchflower und Land-mine

```text
Kernel-pult | Wall-nut | Butter-nut
Kernel-pult | Umbrella Leaf | Umbrella Husk
Sunflower | Torchwood | Torchflower
Potato Mine | Sea-shroom | Land-mine
```

Für jedes Ergebnis enthält das vollständige Quellenarchiv genau dieses eine Rezept. Alle Zutaten waren bereits vor dieser Änderung verfügbar. Die gemeinsame Rezeptauflösung unterstützt beide Kartenreihenfolgen und Handschuhrichtungen, soweit das Gelände die Ergebnisidentität zulässt. Kosten und Cooldown gehören weiterhin zur eingehenden Karte beziehungsweise zum Handschuh. Die Ergebniskennung wird vor der Controller-Erzeugung gesetzt; erst ein erfolgreich erzeugtes Ergebnis verbraucht die Zutaten. Unterlagen und Kürbisse bleiben erhalten. Insbesondere bleibt Land-mine eine Landmine: Das Sea-shroom-Rezept umgeht nicht die native Wasserprüfung des Ergebnisses. Es gibt keine gesperrten Alternativrezepte. Genau vier erledigte Zeilen wurden aus `fusionen.txt` entfernt; Folgefusionen bleiben offen. Jetzt sind 96 Rezepte aktiv; 572 Rezepte für 483 Ergebnisse bleiben offen. `fusionen-quellen.json`, `workflow.md` und `worklfow.md` bleiben unverändert.

Butter-nut besitzt 4000 HP. Jeder tatsächliche Biss betäubt den Angreifer für zwei Sekunden mit Butter, einschließlich des letzten Bisses. Übersprungene Bisse lösen nichts aus. Die gemeinsame Funktion `ButterEffects.Apply` verwendet zuerst die native Butterfunktion für Immunitäten, Grafik und unterbrochene Zombieaktionen. Nur eine tatsächlich angenommene Betäubung erhält die Quelldauer von 200 Simulationsticks; eine bereits längere Betäubung wird nicht verkürzt. Die Originalgrafik enthält alle drei Schadensstufen und beide Controllerclips.

Umbrella Husk besitzt 1000 HP und behält den nativen Schutz vor Bungee-Zombies und Katapultgeschossen. Kontakt mit einem geeigneten Zombie derselben Reihe löst höchstens einmal pro Sekunde einen Puls im Radius von zwei Quellfeldern aus: 40 Schaden, zwei Sekunden Butter und sofortiger Rückstoß um drei Quellfelder. Boss und Fahrzeuge behalten die bestehenden Ausschlüsse. Ohne den noch nicht implementierten Umbrella Pot gelten genau diese Grundwerte; dessen zusätzliche Verstärkung wird nicht vorweggenommen. Wie in der tatsächlichen Basisklasse `CornUmbrella.DamageHealth` kostet ein Puls 100 HP für einen, 150 HP für zwei bis vier oder 250 HP für mindestens fünf getroffene Zombies. Diese Kosten gelten bereits für Umbrella Shell und bleiben dort erhalten. Leerlauf und vollständige Blockanimation stammen aus dem Originalcontroller.

Torchflower besitzt 300 HP und produziert 25 Sonne im nominellen Abstand von 25 Sekunden, mit der vorhandenen belegten Streuung von 23,75 bis 26,25 Sekunden und nativer erster Produktionsverzögerung. Die Produktion erfolgt 500 ms nach Beginn des Helligkeitspulses. Bei jeder Produktion verschwinden zusätzlich alle lebenden pflanzlichen Projektile innerhalb von zwei Quellfeldern; für jedes entsteht an seiner Position ein physischer Pickup mit exakt fünf Sonne. Die tatsächliche Methode `TorchSunflower.ProduceSun` verwendet Radius 2, obwohl der Almanach Radius 1,5 nennt. Zombie-Erbsen, Basketballgeschosse und Projektile fremder Boards werden nicht umgewandelt. Die Flughöhe zählt zum Abstand. Pause, Tragen, Garten und I-Zombie stoppen die Produktion. Der bestehende `SolarShooter.DropSun`-Weg erhält die Sammlung durch Maus und Sun Collector. Torchflower besitzt keine zusätzliche Torchwood-Zündung.

Land-mine besitzt 300 HP und ist sofort bewaffnet. Nach jeweils 29 bis 31 Sekunden wächst die Mine, höchstens zweimal. Die Ausgangsstufe verursacht 900 Schaden, beide höheren Stufen 1800 Schaden. Die beiden ersten Stufen verwenden den ursprünglichen kleinen Minenradius von einem Quellfeld und treffen nur dieselbe Reihe. Die letzte Stufe verwendet `MidBombPotato` mit Radius zwei und darf benachbarte Reihen treffen. Diese tatsächlichen Codewerte gehen über die verkürzte Almanachangabe von 0,74 beziehungsweise 1,5 hinaus. Kontakt und Zerquetschen lösen die Explosion aus; ein gewöhnlicher Tod, Schaufeln und Zutatenverbrauch erzeugen keine zusätzliche Explosion. Die Originalgröße wächst in jeweils 250 ms von 0,15 auf 0,20 und 0,25. Die native Platzierung bleibt der Bewegungsanker; der Kontaktbereich wächst mit dem Originalcollider. Die Originalleuchte blinkt alle gut zwei Sekunden für 100 ms. Dünger setzt den unabhängigen Wachstumstimer dieser bereits bewaffneten Mine nicht zurück.

Keine dieser vier Pflanzen verschießt eigene Fluggeschosse. Butter und Minenexplosionen verwenden native Effekte; die Sonnenumwandlung verbraucht vorhandene Projektile. Deshalb fehlt keine neue Geschossart. Die vollständigen Originalmodelle und sämtliche Controllerclips wurden aus dem lokalen 4.0.5.2-Bundle exportiert und eingebettet: `CornNutPrefab` 49492, Transform 100805, Controller 6582; `CornUmbrellaPrefab` 40547, Transform 117220, Controller 6546; `TorchSunflowerPrefab` 46022, Transform 96764, Controller 6493; `SeaMine_landPrefab` 73148, Transform 96844, Controller 6388. Pivot, Hierarchie, Schatten und Sortierung bleiben erhalten. Unity-Spielskripte werden nicht geladen. Quellenrezepte, chinesischer Almanach, Prefab-/Scriptidentität, aktuelle Methoden und Exportprüfsummen liegen unter `references/butter-nut/`, `umbrella-husk/`, `torchflower/`, `land-mine/`, `butter-nut-behavior/`, `torchflower-base/`, `torchflower-producer/` und `butter-torch-native/`.

Die neuen gespeicherten Paare sind `Wallnut/Kernelpult`, `Umbrella/Kernelpult`, `Sunflower/Torchwood` und `Potatomine/Seashroom`. Gesundheit, Identität, Butterzustand und Produktionstimer bleiben native Speicherfelder. Land-mine speichert Wachstumsstufe in `mStartRow`, verbleibende Größenänderung in `mTargetX`, Wachstumstimer in `mStateCountdown`, Blinkintervall in `mLaunchCounter` und Blinkdauer in `mShootingCounter`. Initialisierung, Reset, Serialisierung und Laden dieser Felder sowie die tatsächlichen Aufrufer sind unter `references/butter-torch-native/` belegt. Nach Laden rekonstruiert die Darstellung diese Identität und den aktuellen Zustand. Die Kennung bereits fallengelassener 5-Sonnen-Pickups bleibt wie beim vorhandenen Solar Shooter flüchtig: Nach Speichern/Laden fällt ein solcher Pickup auf den nativen SmallSun-Wert zurück. Die Pflanzen erzeugen danach wieder korrekt markierte 5-Sonnen-Pickups.

Offline-Nachweise: Rezept-/Assetprüfung, Werkzeugregressionen, betroffene isolierte C#-Regressionen, Originalassetvorschauen und Release-Build. Kein Spielstart, keine Laufzeittests und keine Änderungen an Profilen oder Spielständen. Installation: `Mods/FusionMod.dll`, Version 0.1.52, über `tools/release.py`; Versions- und Binärnachweis in `references/release-0.1.52.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.51.dll` nach `Mods/FusionMod.dll` kopieren. Neue Varianten benötigen beim Laden Version 0.1.52 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten/Cooldown, Vorschau, Hervorhebung, Abbruch, Unterlagen und Terrain. Butter-nut mit normalem, übersprungenem und tödlichem Biss sowie Butterimmunität und längerer vorhandener Betäubung prüfen. Umbrella Husk auf Kontaktradius, Rückstoß, Butter, Eigenkosten, Bungee- und Katapultschutz prüfen. Torchflower auf Produktionsrhythmus, Radiusgrenzen, Flughöhe, gegnerische Geschosse, exakt fünf Sonne pro umgewandeltem Projektil und Sun Collector prüfen. Land-mine sofort nach Fusion sowie nach beiden Wachstumsstufen auf Schaden, Radius, Kontakt, Zerquetschen und gewöhnlichen Tod prüfen. Originalgrafik, alle Schadensstufen, Blockanimation, Wachstum, Pause, Tragen, Tod und Speichern/Laden prüfen.



## Änderung 0.1.51: Foul-nut, Star-nut, Pea Squash und Iron Squash

Alle vier Quellenrezepte sind implementiert: Garlic + Wall-nut, Starfruit + Wall-nut, Peashooter + Squash sowie Squash + Bucket. Beide Reihenfolgen funktionieren über die vorhandene Rezeptauflösung; Bucket stammt aus Magnet-shroom oder einem abgelegten Eisenfund. Alle Zutaten waren vor dieser Änderung verfügbar. Es gibt keine gesperrten Alternativrezepte dieser Ergebnisse. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; Folgefusionen bleiben offen. Das Quellenarchiv und beide Workflowdateien bleiben unverändert.

Foul-nut besitzt 4000 HP. Jeder tatsächliche Biss vergiftet den Angreifer für zehn zusätzliche Sekunden und erhöht seinen Giftwert um eins, höchstens auf 30. Die native gespeicherte Ekelanimation übernimmt den anschließenden Reihenwechsel samt Geländeprüfung. Gift verursacht pro Sekunde 20 Schaden je Giftwert und umgeht die zweite Rüstung. Es nutzt denselben Zustand wie Poison Pea, erzeugt beim Biss jedoch keine Miasmawolke. Die gemeinsame Umsetzung berücksichtigt jetzt den belegten `SetPoison`-Aufruf: Auch der sechste und jeder spätere Erbsentreffer erhöht den Giftwert, statt bei fünf stehenzubleiben. Giftzustände bleiben wie bisher flüchtig und werden beim Laden nicht wiederhergestellt.

Star-nut besitzt 4000 HP und sucht keine Ziele. Ein erfolgreicher Biss, eine Zombie-Erbse, ein Basketballtreffer, ein brennender Zombie-Jalapeno-Kopf, Zerquetschen oder eine Flächenexplosion löst fünf Sterne mit je 20 Schaden aus. Die Originalwinkel sind 30°, −30°, 180°, 90° und −90°; der rückwärtige Stern fliegt mit sechs statt fünf Quellzellen pro Sekunde. Schaufel, Fusionsverbrauch, unterdrückter Schaden und das spätere Entfernen eines bereits tödlich getroffenen Exemplars lösen keinen zusätzlichen Schuss aus. Die Mod beobachtet die tatsächlichen nativen Gesundheitsschreiber. `mLaunchCounter` speichert bei dieser nicht schießenden Wall-nut-Grundidentität den bereits quittierten Gesundheitswert und verhindert doppelte Todestreffer.

Pea Squash nutzt den nativen Squash-Sprung und verursacht beim Landen 1800 Quetschschaden. Anschließend entstehen drei vollständige Erbsenringe mit insgesamt 108 Projektilen, jeweils 20 Schaden. Die Ringe enthalten je 36 Winkel mit zehn Grad Abstand und fliegen mit fünf, sechs beziehungsweise sieben Quellzellen pro Sekunde. Native `ProjectileMotion.Star` übernimmt Flug, Reihenwechsel, Kollision und Speicherung. Die Erbsen behalten native Torchwood-Konvertierung; zusätzliche vertikale Grenzen entfernen auch senkrechte und konvertierte Erbsen außerhalb des Spielfelds.

Iron Squash besitzt 1000 HP und verursacht 3240 Quetschschaden. Ein Bucket heilt über die vorhandenen Magnet-/Fund-/Handschuhwege 500 HP, höchstens bis zum Maximum. Die native Jack-in-the-Box-Explosion entfernt Pflanzen unmittelbar; für Iron Squash wird genau dieser Pfad auf 900 Schaden abgebildet, entsprechend 50 Prozent des ursprünglichen Kirschschadens. Der erste Treffer bei voller Gesundheit lässt 100 HP übrig und zählt die überlebende Pflanze nicht als gefressen. Andere Entfernungen und Quetschschaden bleiben zuständig. Beide neuen Squash-Varianten erfassen Ziele eine halbe Zelle links bis eine Zelle rechts und erhalten native Sprungzustände, Terrainregeln und Speicherfelder.

Die gespeicherten Paare sind `Wallnut/Garlic`, `Wallnut/Starfruit`, `Squash/Peashooter` und `Squash/Magnetshroom`. Sie werden vor der Controller-Erzeugung gesetzt. Die vorhandenen Karten-, Handschuh-, Vorschau-, Hervorhebungs- und Verbrauchswege bleiben gemeinsame Verantwortlichkeit. Seerosen, Töpfe und Kürbisse bleiben erhalten. Fliegende Erbsen und Sterne verwenden ausschließlich native gespeicherte Simulationsfelder; die Originaldarstellung wird nach Laden aus Typ und Bewegung rekonstruiert. Es gibt keine flüchtige Schadens- oder Besitzertabelle für diese Projektile.

Originalassets und Verhalten stammen ausschließlich aus dem lokalen 4.0.5.2-Bestand: GarlicNutPrefab 49500 (WallNut), StarNutPrefab 49516 (StarNut, nicht Tower_starNut), PeaSquashPrefab 68174 (PeaSquash), IronSquashPrefab 68173 (Squash), Bullet_pea 33962 und Bullet_star 33973. Beide Nüsse verwenden Controller 6582 mit zwei Clips; beide Squash-Modelle verwenden Controller 6477 mit allen fünf Leerlauf-, Blick-, Sprung- und Landungsclips. Die Originalhierarchie, Sprites, Pivots, Schadenskörper und Schatten sind eingebettet. Sterne erhalten zusätzlich die originale skriptgesteuerte Drehung ihres sichtbaren Körpers. Native Trefferpartikel und Geräusche ersetzen Unity-Spielskripte und Partikeleffekte. Assetprüfsummen und vollständige Quellenbelege liegen in den gleichnamigen `references/`-Verzeichnissen, unter `references/retaliation-projectiles/` und `references/nut-squash-native/`.

Offline-Nachweise: statische Rezept-/Assetprüfung, Workflowprüfungen, betroffene isolierte Regressionen und Release-Build. Kein Spielstart, keine Spieltests und keine Änderungen an Profilen oder Spielständen. Installation und Versionsnachweis: `references/release-0.1.51.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.50.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit den neuen Varianten benötigen Version 0.1.51 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

Manuelle Prüfung durch den Benutzer: alle vier Rezepte in beiden Karten- und Handschuhrichtungen, Bucket-Auswahl und Fütterung, Kosten/Cooldown, Abbruch, Hervorhebung, Terrain sowie Unterstützungspflanzen. Foul-nut einschließlich letztem Biss, Reihenwechsel, Giftstapelung und Wechselwirkung mit Poison Pea prüfen. Star-nut gegen Bisse, Zombie-Erbsen, Basketball, Explosion und Quetschen prüfen; Schaufeln und Fusion dürfen keine Sterne auslösen. Pea Squash auf 108 Erbsen, Schaden, alle Richtungen, Torchwood und Entfernung am Rand prüfen. Iron Squash auf 3240 Schaden, 500 HP Heilung und 900 Schaden durch Jack-in-the-Box prüfen. Originalgrafik, alle Schadensstufen, Sprunganimationen, Pause, Tod, Pool-Wiederverwendung sowie Speichern/Laden prüfen.

## Änderung 0.1.50: Poison Pea, Poison Repeater, Split Poison und Gatling Poison

```text
Garlic | Peashooter | Poison Pea
Garlic | Repeater | Poison Repeater
Peashooter | Poison Pea | Poison Repeater
Garlic | Split Pea | Split Poison
Peashooter | Poison Repeater | Split Poison
Poison Pea | Repeater | Split Poison
Garlic | Gatling Pea | Gatling Poison
Peashooter | Split Poison | Gatling Poison
Poison Pea | Split Pea | Gatling Poison
Poison Repeater | Repeater | Gatling Poison
```

Alle zehn Quellenrezepte sind umgesetzt. Zuerst war Poison Pea aus den vorhandenen nativen Zutaten verfügbar; danach wurden Poison Repeater, Split Poison und Gatling Poison in dieser Abhängigkeitsreihenfolge implementiert. Damit sind sämtliche Alternativen dieser vier Ergebnisse verfügbar. Beide Reihenfolgen gelten für Karten und Handschuh. Der gemeinsame Fusionsweg erzeugt das Ergebnis mit seiner Speicherkennung vor dem Zutatenverbrauch, erhält Unterlagen und Kürbisse und verwendet die Kosten beziehungsweise den Cooldown der eingehenden Karte oder des Handschuhs. Laden und geskriptete Erzeugung lösen keine Fusion aus. Die zehn erledigten Zeilen sind aus `fusionen.txt` entfernt. Jetzt sind 88 Rezepte aktiv; 580 Rezepte für 491 Ergebnisse bleiben offen. Quellenarchiv und Workflow bleiben unverändert.

### Fähigkeiten und Geschosse

Alle vier Pflanzen haben 300 Lebenspunkte und verwenden die native Zielsuche sowie den normalen Schusszyklus von nominell 1,5 Sekunden. Poison Pea verschießt eine, Poison Repeater zwei und Gatling Poison vier Knoblaucherbsen. Split Poison verwendet dieselbe bereits vorhandene gekoppelte Zielsuche wie die anderen Split-Varianten: ein vorderer Schuss und zwei hintere Schüsse; hintere Erbsen prallen am linken Rand zurück. Die Originalanimationen folgen der nativen Schussphase. Sie erzeugen keine zusätzlichen Geschosse.

Jede Knoblaucherbse verursacht 30 direkten Schaden und gibt dem Ziel anschließend eine Giftstufe. Die tatsächliche Quelle `Zombie.AddPoisonLevel` beginnt mit fünf Stufen; erst der sechste Treffer startet fünf Sekunden Vergiftung. Weitere Treffer verlängern diese Dauer jeweils um fünf Sekunden. Die Vergiftung verursacht sofort beim nächsten Simulationstick und danach jede Sekunde 20 Schaden je Giftstufe, gewöhnlich 100. Gefrieren hält die verbleibende Giftdauer an, aber nicht den Giftschaden. Nach Ablauf werden die Stufen zurückgesetzt. Der Giftschaden und der Wolkenschaden umgehen Schilde; Helme nehmen weiterhin Schaden. Das entspricht dem Quellenweg `ApplyDamage` für Schadensart 4, der die zweite Rüstung überspringt und zuerst die erste Rüstung behandelt. Native Schadenszustände und die vorhandene Überhitzungsverstärkung bleiben erhalten. Verbündete, tote Ziele und fremde Boards erhalten keine neuen Giftstufen. Boss, Grabstein und Zielattrappe behalten direkten Trefferschaden, aber keine Vergiftung.

Ein Treffer auf einen vergifteten Gegner erzeugt an dessen Position eine stationäre Giftwolke. Das gilt auch für den Treffer, der die Vergiftung auslöst, und für einen tödlichen Treffer auf einen bereits vergifteten Gegner. Pro Reihe existiert höchstens eine Wolke. Weitere Treffer verschieben sie nicht und setzen ihre Lebensdauer nicht zurück. Die Wolke besteht sechs Sekunden. Jede Sekunde erhalten geeignete Zombies derselben Reihe innerhalb von 0,74 Feldern eine Giftstufe und Schaden in Höhe des Zwanzigfachen der Schusszahl aller aktiven Gift-Erbsenschützen dieser Reihe: 20/40/60/80 je Poison Pea/Poison Repeater/Split Poison/Gatling Poison. Eine Wolke bleibt nach dem Tod ihres Erzeugers bestehen; ohne aktive Gift-Erbsenschützen verursacht sie keinen zusätzlichen Wolkenschaden, gibt aber weiterhin Giftstufen.

Poison Repeater kann nach 15 Sekunden durch einen normalen Linksklick zu Split Poison wachsen. Split Poison wächst nach weiteren 15 Sekunden auf dieselbe Weise zu Gatling Poison. Der gemeinsame Besitzer `ShooterGrowth` enthält jetzt diesen bereits für die Blaze-Familie vorhandenen Ablauf; die bestehenden Blaze-Regeln bleiben erhalten. Der Countdown liegt im nativ gespeicherten Feld `mStartRow`. Pause, Tragen, Bungee, Garten und Ladephasen stoppen ihn. Erst ein erfolgreich erzeugtes und platziertes Ergebnis verbraucht die vorherige Pflanze; erlittenen Schaden behält die neue Pflanze.

Das Originalgeschoss ist `Bullet_pea_garlic`, Quellen-Bullet-ID 177. Gewöhnliches Torchwood wandelt es nicht um: Der tatsächliche Quellenweg akzeptiert nur gewöhnliche und Eis-Erbsen. `Bullet_firePea_garlic` gehört zu einer anderen, noch nicht verfügbaren Torchwood-Fusion und wird hier nicht fälschlich durch gewöhnliches Torchwood erzeugt. Somit fehlt keine Geschossart dieser vier Pflanzen. Ihre bewegten Geschosse verwenden die Originalsprites und die native Kollisions-, Bewegungs- und Rückprallsteuerung. Giftwolken sind Flächeneffekte, keine zusätzlichen Fluggeschosse.

### Originalassets, Quellen und Speicherung

Grafik und Animationen stammen ausschließlich aus dem lokalen Bundle von `english-4.0.5.2`. Originalobjekte: Poison Pea `GarlicPeaPrefab` 37179, Transform 120497, Controller 6360; Poison Repeater `GarlicRepeaterPrefab` 54442, Transform 110837, Controller 6267; Split Poison `GarlicSplitPrefab` 37180, Transform 120498, Controller 6363; Gatling Poison `GarlicGatlingPrefab` 38419, Transform 107623, Controller 6294; Geschoss `Bullet_pea_garlic` 34023, Transform 117796. Repeater und Gatling benutzen tatsächlich ebenfalls das Script `GarlicPea`; Split benutzt `GarlicSplit`. Alle Sprites und beide Originalclips jeder Pflanze sind eingebettet. Die belegten verwaisten Bindungen der gemeinsam mit Blaze verwendeten Controller werden mit denselben engen Ausnahmen exportiert. Die Assetvorschauen wurden offline visuell geprüft.

Rezepte, lokaler chinesischer Almanach, aktuelle Methodenadressen, Disassemblierung, Prefab-/Scriptidentitäten und Exportprüfsummen liegen unter `references/poison-pea/`, `poison-repeater/`, `split-poison/`, `gatling-poison/`, `poison-projectile/`, `poison-base/`, `poison-effects/`, `poison-status/` und `poison-miasma/`. Die Miasma-Lambdas wurden über die tatsächlichen Metadaten der lokalen DLL aufgelöst. Ihre Zeilenprüfung und die serialisierten Wolkenwerte sind archiviert. Fremde Unity-Spielskripte werden nicht ausgeführt. Die Originalwolke benutzt einen Unity-Partikelemitter; im Port visualisiert der native Replanted-Effekt `FumeCloud` die Wolke bei Erzeugung und jeder Schadenswelle. Auch Einschlag und Geräusche verwenden die nativen Effekte. Das ist ein dokumentierter Effekt-Ersatz, keine Übernahme des ursprünglichen Partikelsystems.

Die neuen eindeutigen Speicherpaare sind `Peashooter/Garlic`, `Repeater/Garlic`, `Splitpea/Garlic` und `Gatlingpea/Garlic`. Identität, Gesundheit, Schussphase und Wachstumscountdown bleiben nativ gespeichert. Die Darstellung wird nach dem Laden aus diesen Kennungen rekonstruiert. Giftstufen, laufende Vergiftung, Giftwolken und die Herkunft bereits fliegender Giftgeschosse sind flüchtiger Modzustand und werden nicht gespeichert. Nach dem Neuladen erzeugte Schüsse besitzen wieder die vollständige Giftfunktion; vorher gespeicherte Fluggeschosse verlieren ihre Gift-Sonderkennung. Board und `DataID` schützen vor Pool-Wiederverwendung. Tod, Reset, Board-Abbau und Mod-Deinitialisierung bereinigen den zugehörigen Zustand; ein zurückgesetzter Boardzähler verwirft alte Giftzustände.

### Offline-Nachweis, Installation und manuelle Prüfung

Release-Build, Rezept-/Assetabgleich, 13 Werkzeugtests und alle zwölf isolierten C#-Regressionen laufen ohne Spielcode. Die neuen Regressionen prüfen alle zehn Rezepte in beiden Reihenfolgen, Speicheridentitäten, direkten Schaden, Giftstufen und Zeitgrenzen, Gefrieren, Giftwolken und Reihenskalierung, Radiusgrenzen, tödliche Treffer, native Geschosse, Kontextwiederherstellung, Pool-Wiederverwendung, Board-Abbau und beide Klick-Upgrades. Die bestehenden Blaze-Regressionen sichern den gemeinsamen Wachstumspfad ab.

Das Spiel wurde nicht gestartet. Profile und Spielstände wurden nicht geändert. Spieltests übernimmt der Benutzer: alle Rezeptalternativen in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten/Cooldown, Vorschau/Markierung, Unterlagen auf Wasser/Dach, Schusszahlen und Split-Rückprall, Schilde/Helme, Vergiftung samt Gefrieren und Überhitzung, Wolkenradius und Skalierung, tödliche Treffer, Klick-Wachstum, Tragen/Pause sowie die dokumentierten Grenzen beim Speichern/Laden.

Installation: `Mods/FusionMod.dll`, Version 0.1.50, über `tools/release.py`. Nachweis: `references/release-0.1.50.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.49.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit den neuen Kennungen erst wieder mit einer passenden Modversion laden. LawnGlove bleibt unverändert. `worklfow.md` verweist weiterhin auf den unveränderten `workflow.md`.


## Änderung 0.1.49: Thorn-nut, Ice Mine, Hypno Mine und Chip Mine

```text
Cactus | Wall-nut | Thorn-nut
Ice-shroom | Potato Mine | Ice Mine
Hypno-shroom | Potato Mine | Hypno Mine
Jalapeno | Potato Mine | Chip Mine
```

Das vollständige Quellenarchiv enthält für jedes dieser vier Ergebnisse genau ein Rezept. Alle benötigten Zutaten waren bereits verfügbar. Karten und Handschuh verwenden beide Reihenfolgen über die bestehende Rezeptauflösung. Ergebnis und Speicherkennung werden vor dem Zutatenverbrauch erzeugt; Seerosen, Töpfe und Kürbisse bleiben erhalten. Kosten und Cooldown stammen weiterhin von der eingehenden Karte beziehungsweise dem Handschuh. Laden und geskriptete Minenstreuung lösen keine Spielerfusion aus. Nur diese vier erledigten Rezeptzeilen wurden aus `fusionen.txt` entfernt; Folgefusionen bleiben offen. Das Quellenarchiv bleibt unverändert.

### Fähigkeiten

- **Thorn-nut:** 4000 Lebenspunkte. Jeder tatsächliche Biss verursacht 50 Gegenschaden, einschließlich des tödlichen Bisses. Übersprungene Bisse verursachen keinen Gegenschaden. Bei fliegenden Ballonzombies innerhalb von 1,5 Feldern derselben Reihe fährt die Pflanze ihren Originalstiel aus. Kontakt zerstört den Ballon über die native Landung. Verbündete, tote Zombies und andere Reihen bleiben ausgeschlossen. Die Originalclips `idle`, `rise`, `idle2` und `unrise` sowie alle drei Schadensstufen werden verwendet. Der besondere Fahrzeugschutz von Spike-nut wird nicht übertragen.
- **Hypno Mine:** 300 Lebenspunkte, niedrige Potato-Mine-Zielkategorie. Die Mine wächst nach dem bestehenden Minentimer von 14 bis 16 Sekunden mit der Originalanimation aus dem Boden. Bewaffneter Kontakt oder Zerquetschen löst die Explosion aus. Geeignete Gegner innerhalb eines Feldradius derselben Reihe werden hypnotisiert; nur bei nicht erfolgreicher Hypnose entstehen 1800 Schaden. Boss, Grabstein und Zielattrappe werden nicht hypnotisiert. Ein gewöhnlicher Tod durch Bisse unter der Erde löst keine Explosion aus. Die Originalgrafik einschließlich der Blinkebene ist eingebunden.
- **Ice Mine:** 300 Lebenspunkte, dieselbe niedrige Zielkategorie und Wachstumszeit. Während des Auftauchens entsteht nach 500 ms eine Eisexplosion: 600 Schaden und vier Sekunden Gefrieren im 3×3-Zellbereich. Bewaffneter Kontakt oder Zerquetschen erzeugt eine weitere Eisexplosion und die gewöhnliche 1800-Schaden-Minenexplosion. Die tatsächliche Quelle `IceMine.Freeze` reicht über die kurze Almanachbeschreibung hinaus: Nach jeweils 150 ms breitet sich die Eisexplosion über benachbarte Zombie-Zellen weiter aus. Jeder Gegner wird je Eisexplosion höchstens einmal getroffen. Separate Minen bleiben unabhängig. Native Gefrierimmunität bleibt erhalten; bereits längeres Gefrieren wird nicht verkürzt. Dünger löst beim sofortigen Auftauchen dieselbe Eisfähigkeit aus, jedoch nicht erneut bei einer bereits bewaffneten Mine.
- **Chip Mine:** 300 Lebenspunkte. `JalaMine` erbt in der Quelle unmittelbar von `Plant`: Die Pflanze bleibt essbar und besitzt keine Auftauch- oder Kontaktzündung. Beim tatsächlichen Tod entstehen eine 1800-Schaden-Minenexplosion und bis zu sieben gewöhnliche Potato Mines in derselben Reihe. Die Suche beginnt rechts, dann links, jeweils mit wachsendem Abstand; die Ursprungszelle bleibt frei. Belegte und ungeeignete Zellen werden übersprungen. Erzeugte Minen benötigen fünf Sekunden bis zum Auftauchen. Die Minenstreuung verwendet denselben Platzierungs- und Kontextschutz wie Cluster Mine; dessen bisheriger 3×3-Bereich bleibt erhalten.

**Keine dieser vier Quellenpflanzen erzeugt eigene Projektile.** Minenstreuung erzeugt Pflanzen. Eis- und Minenexplosionen verwenden native `IceSparkle`-/`Powie`-Effekte und native Explosionsgeräusche. Es fehlen daher keine neuen Fluggeschossarten.

### Quellen, Darstellung und Speicherung

Maßgeblich bleiben `english-4.0.5.2/Game Files/GameAssembly.dll`, das Originalbundle und der lokale Almanach. Rezepte, chinesische Beschreibung, Methodenadressen, Disassemblierung, Prefab-/Scriptidentität, Controller und Exportprüfsummen liegen unter `references/thorn-nut/`, `references/hypno-mine/`, `references/ice-mine/`, `references/ice-mine-coroutines/` und `references/chip-mine/`.

Originalobjekte: Thorn-nut `CactusNutPrefab` 72702, Transform 103542, Controller 6567; Hypno Mine `HypnoMinePrefab` 44432, Transform 105736, Controller 6380; Ice Mine `IceMinePrefab` 44437, Transform 105741, Controller 6381; Chip Mine `JalaMinePrefab` 44155, Transform 114709, Controller 6382. Alle Sprites und Clips werden eingebettet. Ice Mines zusätzliches 200-ms-Blinken verwendet die beiden nachgewiesenen Originalsprites aus dem serialisierten Scriptobjekt 195419 (Renderer 139533, Sprites 20092/11880); der Export bildet diesen Scriptablauf als Datenclip ab. Nur nachgewiesene verwaiste Augenbindungen des von Hypno Mine mitbenutzten Potato-Mine-Controllers werden übersprungen: `PotatoMine_body/PotatoMine_eyes` fehlt im Original-Hypno-Prefab, das stattdessen `PotatoMine_eye1` und `PotatoMine_eyes2` besitzt. Die konkrete Ausnahme steht im Exportmanifest. Die Einzelvorschauen unter den Referenzordnern sind ausschließlich Assetprüfungen.

Neue eindeutige Speicherpaare: `Wallnut/Cactus`, `Potatomine/Iceshroom`, `Potatomine/Hypnoshroom` und `Potatomine/Jalapeno`. Gesundheit, Zustand, Auftauchzeit, Blinken und Thorn-nuts Höhenwechsel verwenden serialisierte native Pflanzenfelder. Die Originalgrafik wird nach dem Laden aus diesen Kennungen rekonstruiert. Board und `DataID` sichern synchrone Biss-, Explosions- und Zielkontexte ab; Zutatenverbrauch und Board-Abbau lösen keine zusätzlichen Explosionen aus. Pause und Tragen stoppen die Fähigkeiten.

Die laufenden Eis-Kettenwellen sind flüchtig: Bereits gespeicherter Schaden und Gefrieren bleiben nativ erhalten, noch ausstehende 150-ms-Folgewellen werden nicht gespeichert. Board-Abbau und Mod-Deinitialisierung entfernen diese Wellen; Rücksprünge des Boardzählers verwerfen alte Wellen. Speichern/Laden während einer Kettenwelle bleibt deshalb ein ausdrücklicher manueller Prüfpunkt.

### Prüfung und Installation

Build, statische Rezept-/Assetprüfungen, Workflowprüfungen und betroffene isolierte C#-Regressionen laufen ohne Laden von Spielcode. Zusätzliche Regressionen prüfen beide Rezeptreihenfolgen, Speicheridentität, echte und übersprungene Bisse, Ballonkontakt, Hypnose und Schadensrückfall, bewaffnetes Zerquetschen, die Eis-Kette mit Pause und Wiederholungsschutz sowie die tatsächliche Minenstreuung mit belegten Zellen und Kontextwiederherstellung bei Fehlern.

Das Spiel wurde nicht gestartet. Spieltests übernimmt der Benutzer: beide Kartenreihenfolgen und Handschuhrichtungen, Vorschau und Markierung, Land/Wasser/Dach mit Unterlagen, Ballons und Schadensstufen, Hypnose einschließlich Gargantuars, Minenwachstum, Dünger, Kettenwellen, Tod/Schaufel sowie Speichern/Laden. Installation erfolgt als Version 0.1.49 über `tools/release.py`; Nachweis: `references/release-0.1.49.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.48.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit den neuen Kennungen erst wieder mit einer passenden Modversion laden. LawnGlove bleibt unverändert. Der vorhandene Workflow wurde inhaltlich nicht geändert; `worklfow.md` verweist auf `workflow.md`.

## Änderung 0.1.48: Solarweed, Iceweed, Scorchweed und Spike-nut

```text
Spikeweed | Sunflower | Solarweed
Ice-shroom | Spikeweed | Iceweed
Jalapeno | Spikeweed | Scorchweed
Spikeweed | Wall-nut | Spike-nut
```

Alle vier Ergebnisse besitzen jeweils genau dieses eine Quellenrezept. Sämtliche Zutaten waren bereits als native Replanted-Pflanzen verfügbar. Alle vier Rezepte gelten in beiden Reihenfolgen für Karten und Handschuh. Die gemeinsame Rezeptauflösung erzeugt das Ergebnis vor dem Zutatenverbrauch; Seerose, Topf und Kürbis bleiben bei der Fusion erhalten. Kosten und Cooldown gehören weiterhin zur eingehenden Karte beziehungsweise zum Handschuh. Laden und geskriptete Erzeugung lösen keine Fusion aus. Die vier erledigten Zeilen wurden aus `fusionen.txt` entfernt. Jetzt sind 64 Rezepte aktiv; 604 Rezepte für 503 Ergebnisse bleiben offen. Das Quellenarchiv bleibt unverändert.

### Fähigkeiten und native Lebenszyklen

- **Solarweed:** 300 Lebenspunkte, 20 Kontaktschaden pro Sekunde. Jeder ausgeführte Angriff erzeugt genau einen einsammelbaren Pickup mit fünf Sonne, unabhängig von der Zahl getroffener Zombies. Ein Angriff, bei dem die Pflanze am Reifen eines Fahrzeugs stirbt, erzeugt weiterhin einen Pickup. Sammlung und Sun Collector verwenden den vorhandenen Adapter für kleine Sonnen; es gibt keine direkte Gutschrift beim Angriff.
- **Iceweed:** 300 Lebenspunkte, 40 Kontaktschaden pro Sekunde. Treffer verursachen zehn Sekunden Kälte und zehn Gefrierpunkte. Die vorhandene gemeinsame Gefriermechanik gilt: zunächst 100 Punkte, danach pro ausgelöstem Gefrieren 100 mehr, höchstens 1000. Gefrieren dauert vier Sekunden; native Immunitäten gelten. Während des Gefrierens entstehen keine weiteren Punkte. Es gibt keinen zusätzlichen Ice-shroom-Schaden und keinen zusätzlichen Schneegeschoss-Schadensmultiplikator.
- **Scorchweed:** 300 Lebenspunkte, 40 Kontaktschaden pro Sekunde. Nach dem Schaden erhalten überlebende geeignete Ziele die bereits implementierte Überhitzung: 50 % mehr eingehender Schaden, Aufhebung durch Kälte, Explosion beim Tod. Native Statusausschlüsse und die bestehenden Kettenexplosionen gelten. Diese Pflanze ist `JalaSpike`, nicht der ähnlich benannte `CaltropTorch` von Torchweed; sie verändert keine durchfliegenden Erbsen.
- **Spike-nut:** 4000 Lebenspunkte, normale hohe Wall-nut-Zielkategorie. Jeder tatsächlich ausgeführte Biss verursacht 50 Gegenschaden; auch der tödliche Pflanzenbiss löst ihn aus. Übersprungene Bisse verursachen keinen Gegenschaden. Bei nativem Überrollen durch Zamboni oder Catapult verliert die Pflanze 500 Lebenspunkte und verursacht den nativen Reifen-Schaden von 1800 am Fahrzeug. Sie bleibt dabei stehen, solange Gesundheit vorhanden ist. Normale Fahrzeuge sterben dadurch. Gargantuar-Hämmer und Bossangriffe behalten ihre normalen Zerstörungsregeln. Die Quelle halbiert außerdem Schaden bei einem Spike Pot auf demselben Feld (`CaltropNut.GetDamage`, Lambda prüft Quell-ID 1372). Spike Pot ist noch nicht implementiert und wird nicht durch einen gewöhnlichen Flower Pot ersetzt.

Alle vier Pflanzen erzeugen **keine eigenen Projektile**. Die drei Bodenpflanzen behalten `SeedType.Spikeweed`: niedrige Zielkategorie, native Angriffsrechtecke, Reichweite, gegnerische Zielauswahl, Schilddurchdringung, normale Gesundheit und Fahrzeugkontakt. Native Spikeweeds und Spikerocks bleiben unverändert. `UpdateAbilities` enthält den Spikeweed-Ablauf bereits inline; sowohl dieser Weg als auch direkte `UpdateSpikeweed`-Aufrufe erreichen `DoRowAreaDamage`. Genau dort werden die 40 Schaden und der synchrone Kontaktkontext eingesetzt. Nur tatsächlich von diesem Spike-Angriff getroffene Gegner erhalten die Zusatzwirkung. Der eigene Kontext wird bei Verschachtelung und Fehlern wiederhergestellt.

Spike-nuts Fahrzeugschutz greift an `Zombie.SquishAllInSquare` und `Plant.Squish`, nach der nativen Prüfung von Reihe, Rechtecküberlappung und erlaubtem Angriff. Board und Pool-ID sichern den Angreifer ab. Der Schutz ersetzt weder Schaufel noch Zutatenverbrauch, gewöhnlichen Tod oder andere Zerstörung. Die Bissvergeltung erweitert den vorhandenen Besitzer für Nuss-Gegenangriffe.

### Originalassets, Speicherung und Grenzen

Grafik, Hierarchie, Pivots und sämtliche Clips stammen aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Solarweed: `SunCaltropPrefab` 64168, Transform 99074, Controller 6222; acht Knoten, fünf Sprites, `idle` 3975 und `attack` 3974.
- Iceweed: `IceCaltropPrefab` 64165, Transform 99071, Controller 6213; sieben Knoten, fünf Sprites, `idle` 3980 und `attack` 3979.
- Scorchweed: `JalaSpikePrefab` 64162, Transform 99068, Controller 6216; acht Knoten, sechs Sprites, `idle` 3964 und `attack` 3963.
- Spike-nut: `CaltropNutPrefab` 67954, Transform 98078, Controller 6582; neun Knoten, vier Sprites, `idle` 4880 und `nothing` 4882. Gesundheit wählt die drei ursprünglichen Schadenskörper; Heilung stellt den passenden Körper wieder her.

Alle vier Controller besitzen genau eine normale Basisebene ohne Skelettmaske. Die Originalangriffsclips der Bodenpflanzen dauern zwei Drittel einer Sekunde; das Ereignis `AnimAttack` liegt bei einem Drittel. Im Port läuft dieser Clip während der letzten Hälfte des nativen Ein-Sekunden-Zyklus, sodass das ursprüngliche Ereignis dem tatsächlichen nativen Kontakt bei Countdown 25 entspricht. Der native Simulationstakt besitzt weiterhin die Treffer. Fremde Unity-Spielskripte und Animationsevents werden nicht ausgeführt.

Die drei Bodenpflanzen besitzen leere Original-`Shadow`-Knoten. Der sichtbare Originalkörper dient deshalb als Größenreferenz; es wird keine fremde Schattengrafik ergänzt. Die neue statische Prüfung erkennt leere Größenreferenzen schon vor der Installation. Nur diese belegte Zeitersparnis wurde als kurzer Hinweis in den Workflow aufgenommen.

Identitäten liegen in den nativen gespeicherten Paaren `Spikeweed + Sunflower/Iceshroom/Jalapeno` und `Wallnut + Spikeweed`. Gesundheit, Angriffsphase und Countdown bleiben nativ gespeichert. Die vorhandenen Gefrierpunkte und Überhitzungsmarkierungen sind flüchtig; ihre bisherigen Speichergrenzen gelten. Bereits erzeugte Fünfer-Sonnen verwenden denselben flüchtigen Herkunftsadapter wie die bisherigen Mod-Sonnen und verlieren beim Neuladen die Sonderwertkennung. Es gibt keinen neuen dauerhaften Mod-Zustand. Pause, Tragen, Bungee, Garten, Laden und Board-Abbau sperren eigene Auslöser.

Quellenbelege liegen unter `references/solarweed/`, `iceweed/`, `scorchweed/`, `spike-nut/`, `spike-nut-effects/`, `spike-nut-lambda/` und `weed-fusions/`: sämtliche Quellenrezepte, lokaler Almanach, aktuelle Methodenadressen und Disassemblierung, Prefab-/Scriptidentitäten, Controller, Assetprüfsummen und Offlinevorschauen. Die Spike-Pot-Abhängigkeit wurde anhand des tatsächlichen Metadaten-Lambdas 918 und dessen Vergleich mit 1372 geprüft; veraltete Proxyadressen sind kein Verhaltensnachweis.

Offline geprüft: 67 neue Assertions mit echten C#-Dateien und Testdoubles; 531 bestehende Assertions; 13 Werkzeugtests; vollständiger Rezept-/Assetabgleich und Release-Build. Die vier Originalvorschauen wurden visuell geprüft. Diese Nachweise laden keinen Spielcode und bestätigen keinen Spieltest. Kein Spielstart; keine Änderung von Profilen oder Spielständen.

Manuell bitte alle vier Rezepte in beiden Kartenreihenfolgen und Handschuhrichtungen, Kosten/Cooldown, niedrige Zielkategorie, Kontakt gegen Gruppen, Fünfer-Sonnen samt Einsammeln, Kälte/Gefrieren/Immunitäten, Überhitzung/Kettenexplosion, erfolgreiche und übersprungene Bisse, normalen und tödlichen Fahrzeugkontakt, Gargantuar/Boss, Schadensgrafiken, Wasser/Dach/Unterlagen und Speichern/Laden während eines Angriffs prüfen.

Installation: `Mods/FusionMod.dll`, Version 0.1.48. Releasebeleg: `references/release-0.1.48.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.47.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.


## Änderung 0.1.47: Flame Wall-nut, Frost Wall-nut und beide Tall-nut-Varianten

```text
Jalapeno | Wall-nut | Flame Wall-nut
Ice-shroom | Wall-nut | Frost Wall-nut
Flame Wall-nut | Tall-nut | Flame Tall-nut
Tall-nut | Jalapeno | Flame Tall-nut
Frost Wall-nut | Tall-nut | Frost Tall-nut
Tall-nut | Ice-shroom | Frost Tall-nut
```

Alle sechs Quellenrezepte sind umgesetzt, jeweils in beiden Reihenfolgen für Karten und Handschuh. Zuerst wurden die beiden Wall-nut-Varianten aus verfügbaren nativen Zutaten implementiert, anschließend die beiden Tall-nut-Varianten. Es gibt keine gesperrten Alternativrezepte für diese vier Ergebnisse. Die sechs erledigten Zeilen sind aus `fusionen.txt` entfernt. Jetzt sind 60 Rezepte aktiv; 608 Rezepte für 507 Ergebnisse bleiben offen. Quellenarchiv und Workflow bleiben unverändert.

Die gemeinsame Rezeptauflösung besitzt weiterhin Platzierung, Hervorhebung und Handschuh. Das Ergebnis entsteht vor dem Zutatenverbrauch; Topf, Seerose und Kürbis bleiben erhalten. Kosten und Cooldown stammen von der eingehenden Karte beziehungsweise den bestehenden Handschuhregeln. Laden und geskriptete Erzeugung lösen keine Fusion aus.

### Fähigkeiten und Originaldarstellung

- **Flame Wall-nut:** 4000 Lebenspunkte. Jeder tatsächlich ausgeführte native Biss verursacht beim Angreifer 20 Gegenschaden und überhitzt anschließend überlebende Angreifer.
- **Frost Wall-nut:** 4000 Lebenspunkte. Jeder erfolgreiche Biss verursacht zehn Sekunden Kälte und zehn Gefrierpunkte. Die vorhandene gemeinsame Gefriermechanik gilt: zunächst 100 Punkte, nach jedem ausgelösten Gefrieren 100 mehr, höchstens 1000. Gefrieren dauert vier Sekunden. Während des Gefrierens entstehen keine weiteren Punkte; native Immunitäten bleiben wirksam. Es entsteht kein zusätzlicher Ice-shroom-Schaden.
- **Flame Tall-nut:** 8000 Lebenspunkte und native hohe Blockade, einschließlich Sprungabwehr. Dieselbe Bissvergeltung wie Flame Wall-nut. Ein Jalapeno-Feuereffekt in ihrer Reihe heilt 3000 Lebenspunkte, begrenzt auf die maximale Gesundheit. Das gilt auch für den vorhandenen Spicy Squash.
- **Frost Tall-nut:** 8000 Lebenspunkte und native hohe Blockade. Dieselbe Bisskälte wie Frost Wall-nut. Ein Ice-shroom-Effekt heilt 1000 Lebenspunkte. Der tatsächliche native `DoSpecial` enthält die Eisexplosion bereits inline; dieser Weg und direkte `IceZombies`-Aufrufe sind berücksichtigt. Gewöhnliche Schneegeschosse und Icy Squash lösen diese Heilung nicht aus.

Überhitzung folgt dem lokalen Mechaniktext und der tatsächlichen `JalaEffect`-Implementierung: 50 % mehr eingehender Schaden, unbegrenzte Dauer, Aufhebung durch erfolgreich angewandte Kälte oder Gefrieren. Sie entfernt vorherige Kälte und Gefrieren. Beim Tod entsteht eine Explosion mit Radius einem Feld, höchstens einer Nachbarreihe, 100 Schaden und Lufttreffern. Die Quelle verursacht zuerst Schaden und überhitzt danach Überlebende. Ein bereits überhitztes Opfer erhält deshalb 150 Schaden und kann bei seinem Tod eine weitere Explosion auslösen; ein zuvor nicht überhitztes, sofort getötetes Opfer erhält keine nachträgliche Todexplosion. Verbündete und ungültige Ziele sind ausgeschlossen. Quelleigene Sonderzombies und Vergiftungszustände existieren in Replanted nicht; Boss, Zielobjekte und Grabsteine erhalten keine neue Überhitzung. Die Effekte verwenden native Schadens-, Kälte- und Explosionsdarstellung; eine eigene rote Zombie-Skin wird nicht eingebunden.

Ausgelassene native Bisse, etwa wegen Leiter oder Kälte-Tick, lösen keine Vergeltung aus. Ein tödlicher Pflanzenbiss löst sie noch aus. Board und Pool-ID sichern verzögerte Rückmeldungen ab. Alle vier Pflanzen besitzen keine eigenen Projektile: Sämtliche vorgesehenen Wirkungen gehören zu Biss, Schaden, Tod oder Sofortpflanzeneffekt. Dünger heilt weiterhin alle Schichten und entwickelt beide Elementar-Wall-nuts nach Ende der Fütterungsanimation zur passenden Tall-nut-Variante. Die gespeicherte Fütterungskennung erhält dabei die Variante; zwischenzeitlich ersetzte oder getragene Pflanzen werden nicht entwickelt.

Grafik, Hierarchie, Pivots, Schadenskörper und Animationen stammen vollständig aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Flame Wall-nut: `FireNutPrefab` 49511, Transform 100824, Controller 6568; elf Knoten, neun Sprites; `idle` 4856 und `fire` 4855.
- Frost Wall-nut: `IceNutPrefab` 77109, Transform 110503, Controller 6572; sieben Knoten, vier Sprites; `idle` 4861.
- Flame Tall-nut: `TallFireNutPrefab` 34405, Transform 88766, Controller 6496; zwölf Knoten, zwölf Sprites; `idle_fire` 4690 und `fire` 4687.
- Frost Tall-nut: `TallIceNutPrefab` 49276, Transform 106174, Controller 6498; sieben Knoten, vier Sprites; `iceidle` 4688.

Die beiden abweichenden Idle-Namen erhalten nur einen Exportalias. Die Feueranimation läuft wie im Original kontinuierlich auf einer additiven Ebene mit Gewicht 1, ohne Skelettmaske, Geschwindigkeit 1. Ihre echten Spritereferenzkurven werden angewendet. Gesundheit wählt zwischen den drei Original-Schadenskörpern; bei Heilung wechselt die Darstellung zurück. Fremde Unity-Spielskripte werden nicht ausgeführt.

### Speicherung, Nachweise und Rückbau

Identitäten liegen in den nativen gespeicherten Paaren `Wallnut/Tallnut + Jalapeno/Iceshroom`. Gesundheit und Fütterung bleiben nativ gespeichert. Überhitzungsmarkierungen sowie die bereits vorhandenen Gefrierpunkte sind flüchtig und gehen beim Neuladen verloren; native Kälte- und Gefrierzeiten bleiben gespeichert. Tod, Reset, Poolwechsel, Board-Ende und Mod-Ende bereinigen Überhitzung. Ein abgebrochener nativer Todesaufruf erhält die Markierung des weiterhin lebenden Zombies. Pausen, Tragen, Bungee, Garten und Board-Abbau lösen keine eigenen Pflanzenaktionen aus.

Quellenbelege liegen unter `references/flame-wall-nut/`, `frost-wall-nut/`, `flame-tall-nut/`, `frost-tall-nut/`, `elemental-nut-effects/`, `elemental-nut-board/` und `elemental-nuts/`: sämtliche Rezepte, lokaler Almanach und Mechaniktext, tatsächliche Methodenadressen/Disassemblierung, Prefab-Scriptidentitäten, Controller-Ebenen, Asset-Prüfsummen und Offlinevorschauen. Insbesondere tragen die Feuerpflanzen die Quellskripte `WallNut` beziehungsweise `TallNutFootball`; Bissvergeltung liegt zentral in `Zombie.EatEffect`, Feuerheilung in `BoardAction.CreateFireLine`. Veraltete Proxyadressen werden nicht als Verhaltensbeleg verwendet.

Offline geprüft: 88 neue Assertions und 443 bestehende Assertions, zwölf Werkzeugtests, neue Rezept-, Identitäts-, Gesundheits-, Biss-, Kälte-, Überhitzungs-, Explosions-, Heilungs- und Lebenszyklusprüfungen mit den echten C#-Dateien und Testdoubles; sämtliche vorhandenen Offline-Regressionen, Werkzeugprüfungen, Assetabgleich und Release-Build. Diese Prüfungen laden keinen Spielcode. Originalvorschauen wurden separat geprüft. Kein Spielstart, keine Änderung von Profilen oder Spielständen. Manuell bitte beide Kartenreihenfolgen, Handschuhrichtungen, Sonne/Cooldown, normale und tödliche Bisse, Leiter, Gefrieren/Immunitäten, Kettenexplosionen, beide Heilungseffekte, Dünger, Schadensgrafiken, hohe Blockade, Dach/Wasser/Unterlagen sowie Speichern/Laden prüfen.

Installation: `Mods/FusionMod.dll`, Version 0.1.47. Releasebeleg: `references/release-0.1.47.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.46.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.

## Änderung 0.1.46: Cherry Squash, Spicy Squash, Icy Squash und Hypno Squash

```text
Cherry Bomb | Squash | Cherry Squash
Jalapeno | Squash | Spicy Squash
Ice-shroom | Squash | Icy Squash
Hypno-shroom | Squash | Hypno Squash
```

Alle vier Ergebnisse haben jeweils genau dieses eine Quellenrezept. Sämtliche Zutaten sind bereits native, verwendbare Replanted-Pflanzen. Karten und Handschuh unterstützen beide Reihenfolgen über die gemeinsame Rezeptauflösung. Das Ergebnis wird vor dem Zutatenverbrauch angelegt; Topf, Seerose und Kürbis bleiben erhalten. Kosten und Cooldowns gehören weiterhin zur eingehenden Karte beziehungsweise zu den vorhandenen Handschuhregeln. Geskriptete Erzeugung und Laden lösen keine Fusion aus. Die vier erledigten Zeilen sind aus `fusionen.txt` entfernt; 54 Rezepte sind aktiv, 614 Rezepte für 511 Ergebnisse bleiben offen. `fusionen-quellen.json` und der Workflow bleiben unverändert.

Alle vier Varianten behalten `SeedType.Squash`, die nativen 300 Lebenspunkte, Sprungbewegung, Trefferzeitpunkt, 1800 Quetschschaden und anschließende Entfernung. Die Quelle ergänzt folgende Fähigkeiten:

- **Cherry Squash:** erweiterte Zielsuche, zwei Felder hinter bis drei Felder vor der Pflanze. Beim Aufschlag kommt die bestehende Kirsch-Flächenexplosion mit 1800 Schaden, Radius zwei Felder und höchstens einer Nachbarreihe hinzu. Wie bei anderen Fusionen mit einer frischen Cherry Bomb löst der gemeinsame Fusionsabschluss außerdem den bisherigen sofortigen Kirscheffekt aus. Ein vorhandener Cherry-nut im Explosionsbereich erhält seine bestehende Heilung.
- **Spicy Squash:** dieselbe erweiterte Zielsuche. Beim Aufschlag erhalten geeignete feindliche Zombies der gesamten Reihe zusätzlich 1800 Explosionsschaden. Kälte und Eisfesseln dieser Ziele werden vorher über `RemoveColdEffects` entfernt. Der Port verwendet begrenzten Schaden; `BurnRow` wäre ungeeignet, weil dessen nativer Aufruf `ApplyBurn` auch Zombies mit deutlich mehr Gesundheit sofort töten kann.
- **Icy Squash:** normale Zielsuche, ein halbes Feld hinter bis ein Feld vor der Pflanze. Nach dem normalen Quetschschaden erhalten geeignete Zombies im Radius von zwei Feldern und höchstens einer Nachbarreihe zusätzlich 600 Schaden und vier Sekunden Gefrieren. Die tatsächliche Methode `IceSquash.AttackZombie` verwendet Radius 2, ein Drittel des Angriffswerts und vier Sekunden; der Almanach beschreibt den Bereich verkürzt als 3×3. Native Gefrierimmunität gilt weiter. Längere vorhandene Gefrierzeiten bleiben erhalten. Der gemeinsame Gefrieradapter setzt anschließend zehn Sekunden Kälte. Die eigenständigen 20 Schaden von `HitIceTrap` werden im eng begrenzten synchronen Kontext unterdrückt; es entstehen ausschließlich die vorgesehenen 600 Zusatzschaden.
- **Hypno Squash:** normale Zielsuche. Jeder vom nativen Quetschpfad tatsächlich getroffene, geeignete Zombie wird mit `StartMindControlled` bekehrt und erhält dann keinen Quetschschaden. Scheitert die Bekehrung oder ist das Ziel ausgeschlossen, bleibt der native 1800-Schaden aktiv. Gewöhnliche Gargantuars sind nicht pauschal immun: Sie erben auch in der Quelle den normalen Bekehrungsweg. Boss, Zielobjekte und Grabstein-Zombies werden nicht bekehrt. Besondere immunisierte Fusion-Zombieklassen besitzen keine zusätzliche Replanted-Entsprechung.

Diese vier Pflanzen erzeugen **keine eigenen Projektile**. Alle vorgesehenen Angriffe erfolgen beim nativen Landetreffer. Die zusätzliche Zielsuche verwendet native Zombie-Rechtecke und Schadenskategorien statt Unity-2D-Collider. Verbündete, tote, nicht erreichbare oder fremden Boards zugehörige Ziele sind ausgeschlossen. Die Zielabstände folgen den Feldangaben des lokalen Almanachs. Die ursprüngliche Physiksuche verwendet quadratische Quellboxen mit Kantenlängen 3 beziehungsweise 8; diese Unity-Koordinaten werden nicht unverändert als Replanted-Felder ausgelegt. Terrain, Absprung, Nachführen des Ziels, Dach und Wasser bleiben im nativen Squash-Ablauf. Explosionspartikel und Audio stammen aus Replanted; die fremde Unity-Physik und fremde Audioverwaltung werden nicht geladen.

### Originaldarstellung und Quellen

Alle Grafiken, Hierarchien, Pivots und fünf Originalclips stammen aus `../pvzfusion/english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Cherry Squash: `CherrySquashPrefab` 68165, Transform 91093, Animator 125697; zehn Knoten, acht Sprites.
- Spicy Squash: `JalaSquashPrefab` 68166, Transform 91094, Animator 125698; sechs Knoten, fünf Sprites.
- Icy Squash: `IceSquashPrefab` 68179, Transform 91107, Animator 125711; sieben Knoten, fünf Sprites.
- Hypno Squash: `HypnoSquashPrefab` 68164, Transform 91092, Animator 125696; acht Knoten, sechs Sprites.

Gemeinsamer Originalcontroller 6477: `idle` 4645, `lookleft` 4648, `lookright` 4649, `jump` 4646, `jumppop` 4647. Die einzelne normale Basisebene besitzt keine Skelettmaske und keine additive Mischung. Alle Bindungen werden exportiert. Der `jump`-Clip enthält zusätzlich eine echte `SpriteRenderer.m_Enabled`-Kurve für den Schatten. Exporter, Darstellung und Offlinevorschau unterstützen diese Kurve jetzt direkt und stellen die Ausgangssichtbarkeit vor jeder neuen Pose wieder her. Vorhandene Modelle ohne dieses optionale Datenfeld behalten ihre bisherige Darstellung.

Die gespeicherten nativen Phasen bestimmen Blickrichtung und Clipfortschritt. Das Quellereignis `AnimMove` bei 0,55 Sekunden und `AttackZombie` bei einem Drittel einer Sekunde sind den nativen Absprung- und Landestellen zugeordnet. Native Zustandszähler lösen weiterhin die Simulation aus; fremde Unity-Events werden nicht ausgeführt. Die native Bewegung besitzt die räumliche Sprungbahn. Deshalb wird die zusätzliche senkrechte Körperverschiebung des Quellclips `jumppop` nicht noch einmal auf diese Bahn addiert; Originalskalierungen, Gesicht und Stielkurven bleiben aktiv. Der Schatten bleibt nach dem Absprung verborgen. Die Quellassets sind unverändert eingebettet, die Laufzeitdarstellung bleibt manuell zu prüfen.

Quellenbelege liegen unter `references/cherry-squash/`, `spicy-squash/`, `icy-squash/` und `hypno-squash/`: lokaler Almanach, vollständige Quellenrezepte, aktuelle Methodenadressen, Disassemblierung, Prefab-/Scriptidentitäten, Animator, Assetprüfsummen und Vorschauen. `references/squash-effects/` und `squash-explosions/` ergänzen `Zombie.SetMindControl`, Explosion und Feuerlinie. Maßgeblich sind die tatsächliche DLL und die aktuelle Metadatentabelle, nicht veraltete Adressen generierter Proxyassemblies. `references/squash-fusions/` archiviert die nativen Aufruf- und Speicherpfade sowie die vollständigen Originalclipdaten. `UpdateAbilities` ruft `UpdateSquash` tatsächlich auf; dessen Landetreffer bei Countdown 5 ruft `DoSquashDamage` auf. Dieser konkrete Weg wird erweitert.

### Speicherung, Prüfung und Rückbau

Die vier Identitäten liegen in `mSeedType/mImitaterType` mit den Markern `Cherrybomb`, `Jalapeno`, `Iceshroom` und `Hypnoshroom`. Gesundheit, Ziel, Sprungphase und Countdown bleiben in den nativen gespeicherten Feldern. Bekehrte Zombies und Gefrierzeiten werden ebenfalls nativ gespeichert. Es gibt keinen zusätzlichen dauerhaften Mod-Zustand. Synchrone Landekontexte werden bei verschachtelten Aufrufen und Fehlern wiederhergestellt. Board und Pool-ID verhindern veraltete Treffer. Pause, Tragen, Bungee, Laden und Board-Abbau sperren die eigenen Auslöser. Schaufel, Zutatenverbrauch und gewöhnlicher Tod lösen keine Landeeffekte aus. Ohne die Mod werden gespeicherte Varianten wieder wie normale Squashes behandelt.

Offline geprüft: 81 neue Assertions für alle Rezeptreihenfolgen, gespeicherte Identitäten, Freigaben und Ausschlüsse, Zielabstände, Flächenschaden, vollständige Feuerreihe, Auftauen, Gefrierimmunität und längere Gefrierzeiten, Bekehrung und Schadensrückfall, Fehler/Verschachtelung, Poolwechsel sowie Animationsphasen. Zusätzlich 362 bestehende Assertions, Werkzeugprüfungen, Rezept-/Assetabgleich und Release-Build. Original-Sprungvorschauen wurden visuell geprüft. Das sind ausschließlich statische und Offline-Nachweise, keine bestätigten Spieltests. Kein Spielstart; keine Änderung von Profilen oder Spielständen.

Manuell bitte beide Zutatenreihenfolgen mit Karte und Handschuh, Sonne/Kosten/Cooldown, Blickrichtung und Sprungdarstellung, Landetreffer, erweiterte Zielsuche vor und hinter der Pflanze, jede Zusatzwirkung gegen Einzelziele und Gruppen, immunisierte Ziele, Rasen/Dach/Wasser, Topf/Seerose/Kürbis und Speichern/Laden während des Sprungs prüfen. Bei Hypno Squash auch einen gewöhnlichen Gargantuar und den Boss prüfen; bei Spicy Squash einen gefrorenen Zombie und einen überlebenden Zombie mit mehr als 1800 Gesundheit außerhalb des Quetschbereichs.

Installation: `Mods/FusionMod.dll`, Version 0.1.46. Nachweis: `references/release-0.1.46.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.45.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.

## Änderung 0.1.45: Twin Marigold

```text
Marigold | Marigold | Twin Marigold
```

Das einzige Quellenrezept ist umgesetzt. Beide Zutaten sind bereits native, verwendbare Marigolds. Karten und Handschuh verwenden die gemeinsame Rezeptauflösung. Kartenkosten und Cooldown stammen von der eingehenden Marigold; die bestehenden Handschuhregeln gelten unverändert. Das Ergebnis wird vor dem Zutatenverbrauch erzeugt. Seerose, Topf und Kürbis bleiben erhalten. Geskriptete Spawns und Laden lösen keine Fusion aus.

Twin Marigold besitzt 300 Lebenspunkte und produziert pro Zyklus zwei unabhängig ausgewürfelte Geldstücke. Je Geldstück gelten 75 % Silber mit Wert 100, 20 % Gold mit Wert 300 und 5 % Diamant mit Wert 1000. Die tatsächliche Quellmethode verwendet Würfe von 0 bis 99: 0–19 Gold, 20–94 Silber, 95–99 Diamant. Das Intervall beträgt 20 Sekunden mit einer Abweichung von ±2 Sekunden. Ein Zyklus beginnt mit Aufleuchten; beide Geldstücke erscheinen nach 0,5 Sekunden. Im Port beginnt der erste Zyklus beim ersten aktiven Fähigkeitstick. Die Pflanze hat keine Angriffe und keine Projektile. Die normalen nativen Marigolds behalten ihre ursprüngliche Produktion und deren Begrenzungen; Twin Marigold verwendet die Produktionsregeln der Quelle ohne native Marigold-Wellenbegrenzung.

Replanted speichert Geld in Zehnern. Der Port schreibt deshalb ausschließlich beim Einsammeln eigener Geldstücke 10, 30 beziehungsweise 100 interne Einheiten gut und korrigiert die Boardstatistik entsprechend. Normale Münzen und Diamanten behalten ihre Werte. Der tatsächliche `Coin.ScoreCoin`-Pfad zerstört zunächst den Pickup und verwendet danach einen inline eingebauten Geldwert. Die Mod erfasst die Herkunft vor diesem Tod und begrenzt die Umrechnung auf den synchronen `UserService.AddCoins`-Aufruf. Finalizer stellen verschachtelte Kontexte auch bei Fehlern wieder her. Pool-ID, Board und Münztyp verhindern eine Übernahme durch fremde Pickups; Tod, Reset, Board-Ende und Mod-Ende räumen die Herkunft auf. Es wird bei Installation kein Geld gutgeschrieben und kein Profil verändert.

Originaldarstellung aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: `TwinMarigoldPrefab` 50942, Transform 114329, Animator 126529, Controller 6540 und Originalclip `idle` 4789. Alle 31 Knoten, 15 Sprites und die vollständige Originalanimation sind eingebettet. Die normale Basisebene ersetzt die Pose; keine additive Ebene, keine Skelettmaske und keine ignorierten Kurven. Das Aufleuchten ist ein separater Helligkeitsverlauf entsprechend `Marigold.<Bright>d__2`; im Port folgt es dem Simulationstakt statt der Bildrate. Die Offlinevorschau wurde visuell geprüft. Sie bestätigt keine Darstellung im Spiel.

Die gespeicherte Identität ist `Marigold + Marigold` in `mSeedType/mImitaterType`. Gesundheit, Produktionsintervall und ausstehende Ausschüttung verwenden native gespeicherte Felder. Laden setzt den Zyklus fort, statt die Pflanze erneut zu initialisieren. Pause, Tragen, Bungee, Board-Abbau, Garten, Ladebildschirm und I-Zombie sperren die Produktion. **Die zusätzlichen Werte bereits ausgeschütteter Geldstücke sind flüchtig:** Nach Neuladen werden diese zu normalen nativen Pickups mit normalen Werten. Vor dem Speichern die eigenen Geldstücke einsammeln. Pflanzenidentität und ausstehende Produktion bleiben gespeichert. Münzgrafik, Flugbewegung und Einsammelgeräusche stammen aus Replanted; die Originalpflanze samt Animation stammt aus PvZ Fusion. Ohne Mod beziehungsweise nach Rückbau wird eine gespeicherte Twin Marigold wie eine normale Marigold behandelt.

Quellen und Nachweise: `references/twin-marigold/` enthält Almanach, vollständige Rezepte, Prefab-/Scriptidentität, Originalcontroller, Assetprüfsummen, Datenassets und Vorschauen. `PlantDataAsset` 187343 belegt 20 Sekunden und 300 Lebenspunkte. `Marigold.Update/MarigoldUpdate/Produce` bei 0x54dba0/0x54d760/0x54d9d0 belegen Intervall, beide Aufrufe und Wahrscheinlichkeiten. Beide verzögerten Aufrufe verwenden denselben nachgewiesenen String `Produce`. `references/twin-marigold-bright/`, `twin-marigold-data/`, `twin-marigold-producer/` und `twin-marigold-native/` ergänzen Coroutine, Grundpflanze und tatsächliche Replanted-Aufruf-/Speicherpfade. Die kontinuierliche Quellenabweichung von ±2 Sekunden wird auf Simulationsticks von 10 ms abgebildet.

Offline geprüft: 37 Szenarioassertionen zu Rezept, gespeicherter Identität, Gesundheit, Wahrscheinlichkeitsgrenzen, zwei unabhängigen Geldstücken, Intervallgrenzen, Aufleuchten, fortgesetzter Ausschüttung nach Laden, Pausen und Ausschlüssen, normaler Marigold, inlinierter Gutschrift, Statistik, Pool-/Boardwechseln, Bereinigung und verschachtelten Fehlerkontexten. Zusätzlich bestehende Regressionen, Werkzeugprüfungen, Quellen-/Assetabgleich und Release-Build. Kein Spielstart und keine Spieltests; keine Änderung von Profilen oder Spielständen.

Manuell bitte Kartenfusion und Handschuhfusion, Kosten/Cooldown, Originaldarstellung und Aufleuchten, beide Geldstücke je Zyklus, Werte beim Einsammeln einschließlich Gold Magnet, Pause/Tragen/Bungee, Seerose/Topf/Kürbis und Speichern/Laden vor der Ausschüttung prüfen. Geldwerte und Laufzeitdarstellung sind offline belegt, aber noch nicht im Spiel bestätigt.

50 Rezepte sind aktiv; 618 offene Rezepte für 515 Ergebnisse bleiben. Nur die erledigte Rezeptzeile wurde aus `fusionen.txt` entfernt. `fusionen-quellen.json` und Workflow bleiben unverändert. Installation: `Mods/FusionMod.dll`, Version 0.1.45. Nachweis: `references/release-0.1.45.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.44.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.


## Änderung 0.1.44: Umbrella Shell

```text
Umbrella Leaf | Wall-nut | Umbrella Shell
```

Das einzige Quellenrezept ist vollständig umgesetzt. Beide Zutaten sind native, bereits verfügbare Pflanzen. Beide Reihenfolgen funktionieren über die gemeinsame Karten- und Handschuhabwicklung. Kosten und Cooldown stammen weiterhin von der eingehenden Karte beziehungsweise den vorhandenen Handschuhregeln. Das Ergebnis entsteht vor dem Zutatenverbrauch; Seerose, Topf und Kürbis bleiben erhalten. Geskriptete Erzeugung und Laden lösen keine Fusion aus.

Umbrella Shell hat 4000 Lebenspunkte. Der native Typ `Umbrella` schützt den eigenen und die acht angrenzenden Plätze gegen Katapultgeschosse und Bungees. Bei Kontakt mit einem geeigneten Bodenzombie derselben Reihe löst die Pflanze höchstens einmal pro Sekunde einen Flächenstoß im Radius von zwei Feldern aus. Jeder geeignete Zombie darin erhält 40 schildumgehenden Schaden und wird unmittelbar um drei Felder zurückgesetzt, begrenzt durch den rechten Boardrand. Fahrzeuge und Boss, verbündete, tote oder nicht erreichbare Zombies sind ausgeschlossen. Der Stoß kostet die Pflanze selbst 100 Lebenspunkte bei einem Ziel, 150 bei zwei bis vier Zielen, 250 ab fünf Zielen. Diese Eigenkosten können die Pflanze töten. Die Kosten sind geerbtes Verhalten von `CornUmbrella.DamageHealth`, nicht im kurzen Almanachtext genannt. Die Variante erzeugt keine eigenen Projektile. Der normale native Schirmschutz bleibt zusätzlich aktiv; normale Umbrella Leaves erhalten weder zusätzliche Gesundheit noch Stoßangriffe.

Originalprefab `NutUmbrellaPrefab` 40549, Transform 117222, Animator 125774 und Controller 6553 stammen aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`. 33 Knoten, 23 Originalsprites und beide Clips `idle`/`block` 4824/4823 sind eingebettet. Die sieben Blätter wechseln zwischen ihren drei originalen Schadensdarstellungen. Die Blockanimation verwendet den Originalclip mit Quellgeschwindigkeit 1,2. Native Abwehraktionen verwenden den Fortschritt der nativen Körperanimation. Der Originalcontroller besitzt eine normale Basisebene ohne Skelettmaske; seine Übergänge sind im Exportbeleg archiviert.

Die Identität `Umbrella + Wallnut`, Gesundheit und beide Fähigkeitstimer liegen in nativen Speicherfeldern. Pool-Wiederverwendung und Board-Ende benötigen keine zusätzliche Zustandsverwaltung. Pause, Tragen, Bungee-Entführung, Board-Abbau und fremde Boards sperren den Stoß. Heilung stellt die passende Blattdarstellung wieder her. Ohne diese Mod beziehungsweise nach Rückbau verhält sich ein gespeichertes Ergebnis wie eine normale Umbrella Leaf.

Quellenbelege: `references/umbrella-shell/` enthält Almanach, alle Rezepte, Prefabkomponenten, tatsächliche Methodenadressen, Disassemblierung, Exportprüfsummen und Vorschauen. Der tatsächliche Verhaltenspfad geht über `CornUmbrella.OnTriggerStay2D/BlockZombies/DamageHealth` und `NutUmbrella.BlockEffect/TakeDamage/ReplaceSprite`. Diese geerbten Methoden liegen zusätzlich unter `references/umbrella-shell-combat/`; Grundpflanze und Zombiebewegung unter `references/umbrella-shell-base/`. Die generierten C#-Signaturen sind nur eine Hierarchiehilfe und besitzen veraltete Adressen; maßgeblich sind ausschließlich die aus der aktuellen DLL ermittelten Methoden. `BungeeLanding` ruft die native `FindUmbrellaPlant`-Suche auf; `UpdateAbilities` enthält den nativen Schirmablauf bereits inline. Die native Suche prüft den Typ `Umbrella`, den diese Fusion dauerhaft behält.

Portierungsgrenzen: Kontaktprüfung und Flächenreichweite verwenden native Replanted-Zielrechtecke und Boardkoordinaten statt Unity-2D-Collider. Der Kontaktbereich entspricht der Breite und X-Verschiebung des Originalcolliders; die Quellphysik wird nicht vollständig simuliert. Quellenrückstoß wird wie in `Zombie.RealKnockBack` unmittelbar angewendet; der Port begrenzt ihn am nativen rechten Boardrand. Der Basisalmanach nennt gerundete 1,5-Felder-Werte; die tatsächlichen Methoden verwenden Radius 2 und Rückstoß 3. Der zusätzliche Quellpfad für `UmbrellaPot` bleibt inaktiv, da diese Unterstützung in Replanted nicht existiert. Native Schirmeffekte und Audio ersetzen Quellaudio. Originalanimation und Sprites sind übernommen; eine identische Laufzeitdarstellung ist noch nicht bestätigt.

Offline geprüft: 36 Szenarioassertionen für Rezeptreihenfolge, gespeicherte Identität und Timer, Flächenwirkung, Rückstoß, Eigenkosten einschließlich tödlicher Kosten, Zielausschlüsse, normale Schirme sowie Pause/Tragen/Board-Ende. Zusätzlich Quellen-/Assetprüfung, Werkzeugprüfungen und Release-Build. Kein Spielstart, keine Spieltests, keine Änderungen an Profilen oder Spielständen. Bitte beide Reihenfolgen mit Karte und Handschuh, Katapult-/Bungeeschutz im 3×3-Bereich, Stoß gegen einzelne und mehrere Zombies, Schadensblätter, Heilung, Topf/Seerose/Kürbis sowie Speichern/Laden prüfen.

49 Rezepte sind aktiv; 619 offene Rezepte für 516 Ergebnisse bleiben. Nur die erledigte Zeile wurde aus `fusionen.txt` entfernt. `fusionen-quellen.json` und Workflow bleiben unverändert. Installation: `Mods/FusionMod.dll`, Version 0.1.44. Nachweis: `references/release-0.1.44.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.43.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.


## Änderung 0.1.43: Darstellung der beschworenen Sonnenkugel

Der Benutzer meldet eine grafische Abweichung der von Twin Solar-nut beschworenen Solar-nut gegenüber PvZ Fusion. In 0.1.42 war zwar der richtige Originalprefab eingebunden, sein Root-Motion-Clip wurde jedoch falsch auf den nativen Projektilanker angewendet. `Round` verschiebt die Wurzel innerhalb von zwei Sekunden von X=0 nach X=8. Der Quell-Animator verwendet ausdrücklich `m_ApplyRootMotion=true`. Der Port spielte diese Verschiebung zusätzlich zur nativen Bewegung als lokalen Grafikversatz ab und setzte sie bei jeder Clipwiederholung zurück. Bei der verwendeten Skalierung entspricht dies einem zusätzlichen Versatz von etwa 422 Board-Pixeln pro Schleife. Beim Übergang vom Warten zum Rollen fiel außerdem die Szenenposition Y=1,189 auf Y=0. Die reine Originalasset-Vorschau verdeckte diese Fehler, weil ihre Kamera dem Schatten folgte.

Die Sonnenkugel verwendet jetzt einen festen Bodenanker: Ihr Originalschatten folgt dem nativen `mShadowY`, auch bei Verstärkung und Terrainwechseln. Die Szenenposition und die zusätzliche Wurzelverschiebung werden nicht als Grafikversatz wiedergegeben. Die Körperrotation stammt weiterhin aus dem originalen `Round`-Clip. Eine eigene, auf der Spielsimulation beruhende Animationszeit beginnt erst beim Rollen bei null; das Alter des Projektils einschließlich Wartezeit bestimmt die Rollphase nicht mehr. Der Quellparameter `Speed` wird berücksichtigt: zunächst 2, danach je Verstärkung um 0,1 weniger bis zur Untergrenze 1. Eine zusätzliche manuelle Projektilrotation entfällt für diese Kugel.

Originalsprites und beide Originalclips bleiben erhalten. Die Korrektur betrifft Darstellung und Animationszeit, einschließlich Bodenverankerung beim Wachstum. Die aus 0.1.42 dokumentierten Bewegungs-, Schadens-, Rezept- und Speichergrenzen gelten weiterhin; insbesondere verlieren vorhandene Sonnenkugeln beim Neuladen ihre Mod-Zuordnung. Andere Pflanzen und Rollgeschosse behalten ihre bisherige Darstellung. Vollständige Laufzeitgleichheit mit PvZ Fusion ist damit noch nicht bestätigt.

Nachweise: `references/solar-nut-ball/rendering-0.1.43.json`, Original-Animator 126525, Controller 6566 und `BigSunNut.Awake/Larger`. `tools/preview_snow_repeater.py --plant solar-nut-ball --runtime-projectile` erzeugt einen Offline-Vergleich mit festem Projektilanker unter `references/solar-nut-ball/runtime-adapter-comparison.png` sowie eine Vorschau-GIF. Diese Vorschau bildet den Adapter nach und ist kein Spieltest. Das neue Szenario prüft Animationsstart, Unabhängigkeit vom Alter des Projektils, Verstärkung, Mindesttempo und Terrainanker. Die Werkzeugprüfung belegt zusätzlich die tatsächlich im Original vorhandene Root Motion und den Speed-Parameter.

Kein Spielstart, keine Spieltests, keine Änderung von Spielständen oder Profilen. Bitte die beschworene Solar-nut im Stillstand, beim ersten Losrollen, über mehrere Rollschleifen und nach mehreren schnellen Verstärkungen prüfen; zusätzlich einmal auf dem Dach. Für weitere sichtbare Abweichungen sind die genaue Stelle und die Art der Abweichung weiterhin hilfreich.

Installation: `Mods/FusionMod.dll`, Version 0.1.43. Nachweis: `references/release-0.1.43.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.42.dll` nach `Mods/FusionMod.dll` kopieren. Die installierte LawnGlove-DLL bleibt unverändert. Rezepte und offene Liste bleiben bei 48 aktiven und 620 offenen Rezepten für 517 Ergebnisse.


## Änderung 0.1.42: Chomp-mine und Sonnenfusionen, sofortiges Kartoffelwachstum

Vier weitere Rezepte sind umgesetzt; Karten und Handschuh verwenden dieselbe Rezeptauflösung und unterstützen beide Zutatenreihenfolgen:

```text
Chomper | Potato Mine | Chomp-mine
Chomper | Sunflower | Solar Chomper
Sunflower | Wall-nut | Solar-nut
Solar-nut | Wall-nut | Twin Solar-nut
```

Solar-nut wird vor Twin Solar-nut verfügbar gemacht. Das zweite Quellenrezept `Twin Inferno-nut | Sunflower | Twin Solar-nut` bleibt ausdrücklich offen: Twin Inferno-nut ist noch nicht implementiert. Es wird keine Ersatzpflanze für diese Zutat akzeptiert. Nur die vier erledigten Rezeptzeilen wurden entfernt. Aktuell sind 48 Rezepte aktiv; 620 Rezepte für 517 Ergebnisse bleiben offen. Das Quellenarchiv bleibt unverändert.

**Chomp-mine:** 300 Lebenspunkte, 75 Sonne unmittelbar nach erfolgreicher Fusion, native Chomper-Zielauswahl und Schlucken mit auf ein Feld begrenzter Reichweite. Die Verdauung dauert 20 Sekunden. Nach abgeschlossener Verdauung entsteht eine normale, sofort aufgerüstete Potato Mine ein Feld voraus, sofern dieses Feld gültig und frei ist. Am rechten Rand, auf unzulässigem Terrain oder bei belegtem Feld entsteht keine Mine; vorhandene Pflanzen werden nicht entfernt. Beim regulären Tod explodiert Chomp-mine mit 1800 Schaden im Radius eines Feldes. Zutatenverbrauch und Board-Abbau lösen keine Explosion aus. Chomp-mine selbst ist bereits aktiv und besitzt keine unterirdische Aufrüstphase.

**Solar Chomper:** 300 Lebenspunkte, native Chomper-Angriffe und 40 Sekunden Verdauung. Erst der erfolgreiche Abschluss der Verdauung erzeugt vier normale Sonnen, insgesamt 100. Fehlbisse, große nicht verschluckbare Ziele und das bloße Beginnen der Verdauung erzeugen diesen Bonus nicht.

**Solar-nut:** 4000 Lebenspunkte, 25 Sonne alle ungefähr 15 Sekunden; nach der ersten Produktion schwankt das Intervall wie bei den vorhandenen Solar-Minen um ±2 Sekunden. Die mittlere und untere Schadensstufe geben jeweils einmal zusätzlich 25 Sonne, der Tod gibt 50 Sonne. Die originalen drei Schadenskörper folgen der aktuellen Gesundheit. Die Quellgrenzen sind streng: mittlere Stufe unter 2666 und über 1333, untere Stufe unter 1333. Übersprungene Stufen werden nicht nachgezählt. Heilung setzt verbrauchte Boni nicht zurück.

**Twin Solar-nut:** übernimmt Produktion, Schadensstufen und Todesbonus von Solar-nut. Ein erfolgreicher nativer Biss, ein Zombie-Erbsentreffer, Zerquetschen oder eine Explosion über den nativen Pflanzenradiuspfad verursacht stattdessen 50 Schaden und erzeugt eine Sonne mit Wert 15. Eine verfehlte Schadensprüfung erzeugt keinen Bonus. Native Biss- und Projektilpfade schreiben Gesundheit direkt; die Umrechnung schützt deshalb bereits vor einem vorzeitigen nativen Tod und stellt reservierte Gesundheit auch bei Fehlern wieder her. Andere direkte Entfernungen, etwa Schaufel und Bungee, bleiben native Entfernungen.

Ein normaler Linksklick auf Twin Solar-nut kostet 500 Sonne, heilt um 4000 Lebenspunkte, auch über die Maximalgesundheit hinaus, und erzeugt eine große Sonnenkugel ein Feld voraus. Vor dem Kostenabzug muss die Kugel erfolgreich erzeugt oder verstärkt werden können. Bei belegtem oder ungültigem Vorderfeld und am rechten Rand bleiben Sonne und Gesundheit unverändert. Weitere Klicks innerhalb einer Sekunde verstärken dieselbe ruhende Kugel und starten diese Wartezeit erneut. Danach rollt sie geradeaus mit dem vorhandenen Rolladapter (1,25 Board-Pixel pro Tick nach dem ersten Bewegungstick) und trifft geeignete Bodenzombies alle zwei Simulationsticks: zunächst 600 Schaden je Treffer. Verstärkungen erhöhen den Schaden um `max(60, floor(600 * exp(-0.3 * (n - 1))))` und den ursprünglichen Maßstab von 0,3 um `0.1 * exp(-2 * Maßstab)`. Native Erbsenkollision und Torchwood-Umwandlung sind für diese Kugeln ausgeschaltet. Die originalen Clips `idle_big` und `Round` stellen Warten und Rollen dar. Die zusätzlichen Reiseupgrades für 10 statt 50 Schaden und 200 statt 500 Sonnenkosten werden nicht aktiviert; der Port besitzt keinen entsprechenden Upgradezustand.

**Halbe Höhe:** Chomp-mine bleibt für KI, Schlucken und Speicherung ein Chomper. Ausschließlich bei `ZombiePea`-Zielsuche wird sie vorübergehend als niedrige Potato Mine behandelt; ein Finalizer stellt ihren Typ mit Board-/DataID-Prüfung wieder her. Die native Suche, Kollisionsgrenze und Kürbispriorität bleiben erhalten. Peashooter-/Gatling-Zombie-Erbsen können Chomp-mine ohne Kürbis nicht treffen. Solar Chomper und beide Sonnen-Nüsse bleiben normal hoch. Die bestehenden niedrigen Kartoffelminen behalten ihre bisherige Kategorie.

**Dünger:** Auf dem ausgewählten Feld werden eingegrabene normale Potato Mines, Peatato Mines und Solar Mines sofort aufgerüstet, auch während des Aufstehens und unter einem Kürbis. Heilung und Wachstum erfolgen bei Anwendung, nicht erst nach der Düngeranimation. Für normale Kartoffelminen wird der native Abschluss des Aufstehens aufgerufen: Dieser tauscht den eingegrabenen Controller und richtet den bewaffneten Controller samt Licht ein. Nur den Zustand `PotatoArmed` zu setzen würde die eingegrabene Darstellung zurücklassen. Fusionen behalten ihre Identität und Fähigkeitstimer. Bucket, Cluster Mine, Mine-nut und Chomp-mine besitzen keinen entsprechenden eingegrabenen Wachstumszustand und erhalten kein erfundenes Wachstum. Andere Dünger-Upgrades behalten ihren bisherigen Ablauf.

### Originalassets, Speicherung und Prüfung

Die Darstellung stammt ausschließlich aus `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`, Datei `resources.assets`. Prefab/Transform/Controller: Chomp-mine `PotatoChomperPrefab` 56013/107987/6625, Solar Chomper `SunChomperPrefab` 32211/94180/6634, Solar-nut `SunNutPrefab` 54016/102846/6636, Twin Solar-nut `SuperSunNutPrefab` 42902/116351/6534, Sonnenkugel `BigSunNutPrefab` 43517/105463/6566. Der englische Name Twin Solar-nut bezeichnet also `SuperSunNut`, während `BigSunNut` dessen Kugel ist. Alle originalen Pflanzenclips, Originalsprites, Schadenskörper und beide Kugelclips sind eingebettet. Die Kugel besitzt im Originalclip 4881 zwei verwaiste Transformpfade; ausschließlich diese nachgewiesenen Pfade werden beim Export ignoriert. Quellenalmanach, vollständige Rezepte, Methodenadressen, Disassemblierung, Komponenten, Exportprüfsummen und Offlinevorschauen liegen unter `references/chomp-mine/`, `solar-chomper/`, `solar-nut/`, `twin-solar-nut/`, `solar-nut-ball/` und `new-fusions-native/`.

Die gespeicherten Identitäten sind `Chomper + Potatomine`, `Chomper + Sunflower`, `Wallnut + Sunflower` und `Wallnut + Twinsunflower`. Gesundheit, Verdauung, Produktion und einmalige Schadensflags liegen in nativen Feldern; das Speicherformat bleibt unverändert. Fliegende beziehungsweise wartende Sonnenkugeln verwenden den bestehenden flüchtigen Rolladapter: Beim Neuladen verlieren sie Herkunft, Verstärkung und Wartezeit und werden native Erbsen. Nicht während einer wartenden oder fliegenden Sonnenkugel speichern. Die neue 15er-Sonne nutzt denselben gegen Board-/Poolwechsel geschützten Wert- und Einsammelpfad wie die vorhandene 5er-Sonne. Originalaudio und zusätzliche Quellpartikel werden durch vorhandene Replanted-Effekte ersetzt. Terrain und Zielrechtecke kommen aus Replanted statt aus Unity-2D-Physik; diese Portierung beansprucht keine identische Physiksimulation.

Offline geprüft: Quellen-/Assetabgleich, 49 neue Szenarioassertionen mit Testdoubles, 87 bestehende Minen-Assertions, 57 bestehende Chomper-Assertions, zehn Werkzeugprüfungen und Release-Build. Kein Spielstart, keine Spieltests, keine Änderung von Profilen oder Spielständen. Manuell bitte beide Reihenfolgen mit Karte und Handschuh, Kosten und Rückerstattung, alle vier Darstellungen, Schadensstufen und Heilung, Verdauungsabschluss, Minenentstehung vor freiem/belegtem Feld und am rechten Rand, Dünger vor/während Aufrüstung, Topf/Seerose/Kürbis und Speichern/Laden prüfen. Zusätzlich Chomp-mine gegen Zombie-Erbsen ohne/mit Kürbis sowie Twin Solar-nuts Biss-, Projektil-, Quetsch-, Explosions- und Klickverhalten prüfen.

Installation: `Mods/FusionMod.dll`, Version 0.1.42. Nachweis: `references/release-0.1.42.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.41.dll` nach `Mods/FusionMod.dll` kopieren. Die installierte LawnGlove-DLL bleibt unverändert.

## Änderung 0.1.41: Nahkampfschaden an niedrigen Minen

Solar Mine und Mine-nut konnten in 0.1.40 endlos angebissen werden, ohne Gesundheit zu verlieren. Peatato Mine hatte dieselbe Sperre nach Beginn des Aufstehens. Die Ursache lag im nativen `Zombie.EatPlant`: Für `SeedType.Potatomine` beendet Replanted den Biss vor dem Gesundheitsabzug, sobald der Pflanzenzustand nicht mehr `Notready` ist. Die vorherige Prüfung der Zielauswahl und Projektilkollision allein erfasste diese zweite Sperre nicht.

Alle drei Fusionsminen nehmen jetzt den regulären nativen Biss-Schaden an. Ausschließlich während `Zombie.EatPlant` wird für diese Varianten der essbare Zustand `Notready` bereitgestellt. Ein Finalizer stellt den vorherigen Zustand danach auch bei Fehlern wieder her. `DataID` und Board schützen vor einer Wiederherstellung auf wiederverwendeten Objekten. Gesundheit, Schadensmenge, Bissfrequenz und tödliche Bisse bleiben im nativen Ablauf. Dadurch können auch Mine-nuts Schadensstufen und die jeweiligen Todeseffekte regulär erreicht werden. Aufrüstung, Fähigkeitstimer und gespeicherte Identität werden nicht zurückgesetzt. Vorhandene Spielstände benötigen keine Umwandlung; der Patch greift auch bei bereits vorhandenen Minen.

Die Pflanzenart bleibt während des gesamten Bisses `Potatomine`. Pflanzen-Zombie-Erbsen fliegen weiterhin durch diese niedrigen Pflanzen. Normale Potato Mines und andere Pflanzen behalten ihre bisherigen Bissregeln. Die bisherigen Kontaktexplosionen von aufgerüsteter Peatato Mine und Solar Mine bleiben erhalten.

Quellenbeleg: `references/mine-bite-fix/`. `Zombie.CheckIfPreyCaught` springt bei `0x6ea274` tatsächlich in `Zombie.EatPlant` (`0x6f0eb0`). Dessen Abbruchprüfung steht bei `0x6f0fd8`–`0x6f0fe5`; der reguläre Abzug von vier Lebenspunkten folgt erst bei `0x6f10d6`. Die Recherche prüft den echten Tail-Jump, nicht nur gewöhnliche `call`-Anweisungen. Die Offline-Prüfung reproduziert zunächst die native Sperre und prüft anschließend alle drei Fusionen vor, während und nach der Aufrüstung, Zustandserhalt, tödliche Bisse, Schadensstufen, normale Potato Mines, verschachtelte Aufrufe, Fehler sowie Pool- und Boardwechsel.

Kein Spielstart und keine Spieltests. Bitte Solar Mine, Mine-nut und Peatato Mine erneut gegen normale Zombies prüfen, einschließlich Mine-nuts Schadensbildern und Explosionen. Zusätzlich den Durchflug von Peashooter-/Gatling-Zombie-Erbsen ohne Kürbis prüfen. Statische Prüfungen und ein Offline-Test mit nachgebildeter nativer Sperre ersetzen diesen Laufzeittest nicht.

Installation: `Mods/FusionMod.dll`, Version 0.1.41. Nachweis: `references/release-0.1.41.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.40.dll` nach `Mods/FusionMod.dll` kopieren. Rezepte und offene Liste bleiben bei 44 aktiven und 624 offenen Rezepten für 520 offene Ergebnisse.


## Änderung 0.1.40: Peatato Mine, Solar Mine und Mine-nut

Die drei Ergebnis-Fusionen sind vollständig über ihre jeweils einzige Rezeptregistrierung aus Fusion 4.0.5.2 erreichbar. Beide Reihenfolgen funktionieren mit Zutatenkarten und Handschuh. Sämtliche Zutaten sind bereits native Replanted-Pflanzen:

```text
Peashooter | Potato Mine | Peatato Mine
Potato Mine | Sunflower | Solar Mine
Potato Mine | Wall-nut | Mine-nut
```

**Peatato Mine:** 300 Lebenspunkte; bei erfolgreicher Fusion werden 50 Sonne unmittelbar gutgeschrieben. Sie schießt bereits unter der Erde Kartoffelerbsen mit 30 Schaden. Der Abstand zum vordersten geeigneten Zombie derselben Reihe bestimmt das Intervall: 1,5 Sekunden ab sechs Feldern, linear bis 0,5 Sekunden bei höchstens einem Feld. Eine Quell-Einheit entspricht hier 80 Board-Pixeln. Nach 14–16 Sekunden beginnt die originale 0,833-Sekunden-Aufstehbewegung. Anschließender Bodenkontakt detoniert die Mine mit 1800 Schaden im Radius eines Feldes. Jeder reguläre Tod streut zusätzlich 36 Kartoffelerbsen im Abstand von zehn Grad in alle Richtungen; diese können andere Reihen treffen. Die Erbsen durchdringen nicht, verursachen keine zusätzliche Flächenexplosion und werden von Torchwood nicht umgewandelt.

**Solar Mine:** 300 Lebenspunkte; bei erfolgreicher Fusion werden 25 Sonne unmittelbar gutgeschrieben. Auch ohne Zombie produziert sie normale 25er-Sonnen. Die erste Produktion erfolgt nach 15 Sekunden, weitere Intervalle schwanken wie in `SunMine.ProducerUpdate` um ±2 Sekunden. Aufrüstung und Kontaktexplosion entsprechen Peatato Mine. Jeder reguläre Tod gibt zusätzlich zwei normale Sonnen, insgesamt 50. Wird sie vor der Aufrüstung gefressen, gibt es diese Sonnen, aber keine Kontaktexplosion.

**Mine-nut:** 4000 Lebenspunkte, sofort als essbare niedrige Barriere aktiv. Zombie-Kontakt löst keine Selbstzerstörung aus. Beim ersten Aufenthalt in der mittleren Schadensstufe und beim ersten Unterschreiten der unteren Schadensgrenze entsteht je eine Explosion mit 500 Schaden. Die Grenzen sind wie im Quellcode streng: bei 4000 Maximalgesundheit liegt die mittlere Stufe unter 2666 und über 1333, die untere unter 1333. Ein großer Treffer direkt in die untere Stufe löst nur deren Explosion aus; die Quelle zählt übersprungene Stufen nicht nach. Heilung setzt ausgelöste Stufen nicht zurück. Beim Tod entsteht eine Explosion mit 1800 Schaden. Die drei originalen Körperdarstellungen folgen der aktuellen Gesundheit.

**Halbe Höhe:** Alle drei Ergebnisse verwenden ausdrücklich `SeedType.Potatomine`, auch Mine-nut. Die Marker `Peashooter`, `Sunflower` und `Wallnut` unterscheiden die gespeicherten Identitäten. Die native Kollision von `ZombiePea` überspringt Potato Mines anhand dieser Pflanzenart. Ein Peashooter-/Gatling-Zombie kann deshalb darüber hinwegschießen. Die normale Nahkampf-, Unterlagen- und Kürbisbehandlung bleibt nativ; ein vorhandener Kürbis ist selbst weiterhin ein hohes Ziel. Keine Größenänderung am Sprite wird als Ersatz für diese Zielkategorie verwendet.

Die gemeinsame Fusionsabwicklung erzeugt das Ergebnis vor dem Zutatenverbrauch. Karten behalten Kosten und Cooldowns ihrer eingehenden Zutat. Der Handschuh behält seine Regeln. Ungültiges Terrain verändert keine Zutaten. Topf, Seerose und Kürbis werden erhalten. Zutatenverbrauch, fehlgeschlagene Erzeugung, Pool-Reset und Board-Abbau lösen keine Todeserbsen, Todesexplosionen oder Todessonnen aus. Rückerstattungen gehören ausschließlich zum erfolgreichen Fusionsabschluss, nicht zur Pflanzeninitialisierung oder zum Laden.

### Originalassets und Nachweise

Grafik, Hierarchie, Pivots, Spritereihenfolge und Originalkurven stammen ausschließlich aus `../pvzfusion/english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Peatato Mine: `PeaMinePrefab` 71626, Transform 99523, Controller 6621; 19 Knoten, 18 Sprites; Clips `idle`, `rise`, `armed`, `mashed`, `shoot` 4973/4975/4972/4974/4976. Die additive Schussebene hat Gewicht 1 und keine Skelettmaske.
- Solar Mine: `SunMinePrefab` 33164, Transform 113828, Controller 6635; 21 Knoten, 22 Sprites; Clips `idle`, `rise`, `armed`, `mashed`, `flash` 5017/5019/5015/5018/5016. Originale Spritewechsel der Blinkebene werden separat angewendet und danach zurückgesetzt.
- Mine-nut: `PotatoNutPrefab` 46252, Transform 98297, Controller 6626; 20 Knoten, 18 Sprites; Clips `armed`/`flash` 4985/4986. `idle` im exportierten Modell ist nur ein Alias des originalen `armed`-Clips.
- Kartoffelerbse: `Bullet_potato` 57125, Transform 118943; zwei Knoten, zwei Originalsprites, kein Animator. Das tatsächliche Script heißt `Bullet_puffPotato`. Das gleichnamige Objekt 60844 ist dagegen ein `PotatoSeed` und kein Geschoss dieser Fusion. `Bullet_potato_explode` 57126 ist ebenfalls eine andere Variante und wird nicht eingebunden.

Quellen, chinesischer Originalalmanach, tatsächliche Methodenadressen, Prefab-/Scriptzuordnung, Exportprüfsummen und Offlinevorschauen liegen unter `references/peatato-mine/`, `references/solar-mine/`, `references/mine-nut/` und `references/potato-projectile/`. Ergänzende Basisklassen und die abweichende Samenidentität sind unter `references/peatato-bullets/` und `references/potato-projectile-identity/` archiviert. Rezeptregistrierungen: `0x47cffa`, `0x47d0f6`, `0x47d10e`.

### Speicherung, Grenzen und manuelle Prüfung

Identität, Gesundheit, Aufrüstung, Produktions-/Schusszähler und einmalige Mine-nut-Schadensflags werden in bestehenden nativen Pflanzenfeldern gespeichert. Die Mod verändert das Speicherformat nicht. Die Markierung benötigt diese oder eine kompatible neuere Mod-Version. Bereits fliegende Kartoffelerbsen verlieren beim Laden ihre flüchtige Mod-Herkunft und werden native Erbsen; neue Schüsse und Todesringe verwenden wieder das Kartoffelverhalten. Beim Tod, Pool-Reset, Board-Ende und Mod-Ende wird die Geschosszuordnung bereinigt.

Die Portierung verwendet Replanted-Zombierechtecke und Board-Terrain anstelle der Unity-2D-Physik der Quelle. Die Kartoffelerbsen bewegen sich mit den aus sechs Quell-Einheiten pro Sekunde abgeleiteten 4,8 Board-Pixeln pro Simulationstick. Schüsse entstehen beim Simulationstakt der Schussauslösung; das Unity-Animationsevent bei 0,667 Sekunden wird nicht ausgeführt. Schuss- und Blinkanimationen sind Darstellungsdaten. Explosionspartikel und Audio kommen aus Replanted; es bleibt kein zusätzliches animiertes Kartoffelbrei-Objekt liegen. Der Originalclip `mashed` bleibt archiviert und eingebettet. Fusion-eigene Zombiearten und deren besondere Minenreaktionen werden nicht ergänzt.

Offline geprüft: 51 neue Szenarioassertionen zu Rezepten, niedriger gespeicherter Identität, Rückerstattung, Aufrüstung, Pause, Zielauswahl, Schusstakt, Produktion, Schadensgrenzen, Heilung, Speicherung der Flags, Todesring, Reihenwechsel, Geschossverbrauch und Pool-/Board-Bereinigung; 147 bestehende Assertions; zehn Werkzeugprüfungen; statische Rezept-/Assetprüfung und Release-Build. Kein Spielstart, keine Spieltests. Bitte beide Zutatenreihenfolgen mit Karte und Handschuh, Sonne/Kosten, Rasen/Dach/Wasser, Topf/Seerose/Kürbis, Schussflug und Todesring, Originalanimationen, Mine-nut-Schadensbilder und Heilung, Speichern/Laden sowie Peashooter-/Gatling-Zombies prüfen. Beim Höhentest einmal ohne, einmal mit Kürbis prüfen.

Die drei erledigten Ergebniszeilen sind aus `fusionen.txt` entfernt; spätere Rezepte mit diesen Fusionen als Zutaten bleiben offen. Aktueller Stand: 44 aktive Rezepte, 624 offene Rezepte für 520 Ergebnisse. `fusionen-quellen.json` bleibt unverändert.

Installation: `Mods/FusionMod.dll`, Version 0.1.40. Nachweis: `references/release-0.1.40.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.39.dll` nach `Mods/FusionMod.dll` kopieren. Die installierte LawnGlove-DLL wird nicht verändert.


## Änderung 0.1.39: Eimer-Drops beim Helmverlust

Normale Bucketheads haben jetzt gemäß der ausdrücklich gewünschten Regel eine **50-%-Chance auf einen Eimer**, wenn ein Schadenstreffer ihren noch vorhandenen Eimerhelm entfernt und sie den vollständigen Treffer überleben. Derselbe Helmverlust würfelt nur einmal. Bloßer Teilschaden und spätere Treffer auf den bereits ungeschützten Zombie erzeugen keinen Drop. Instant-Kills, Verschlucken, Zerquetschen und Verbrennen erzeugen keinen solchen Eimer; auch ein gewöhnlicher Treffer, der Helm und Körper gleichzeitig tötet, ist ausgeschlossen. Andere Helmtypen und verbündete Zombies erzeugen keine Eimer.

**Magnetentzug bleibt erhalten:** Magnet-shroom nimmt weiterhin genau seinen nativen Eimer auf. Diesen Magnet-shroom anklicken und den Eimer wie bisher pflanzen, fusionieren oder zum Heilen verwenden. Der Magnetentzug erzeugt keinen zweiten Boden-Drop und keinen zusätzlichen Zufallswurf. Der native Magnetweg entfernt den Helm direkt und ruft den beobachteten Schadenstreffer nicht auf.

Ein erfolgreicher Schadens-Drop erscheint am Zombie, fliegt kurz hoch und fällt zu Boden. Die sichtbare Grafik ist das Originalsprite 15928 des losen Fusion-Eimerprefabs 59960. Der Eimer bleibt acht Simulationssekunden liegen und blinkt nach vier Sekunden. Anklicken nimmt ihn an den bestehenden Eimercursor. Während der Auswahl laufen weder Lebensdauer noch Flug weiter. Rechtsklick oder Werkzeugwechsel legt denselben Eimer mit seiner restlichen Lebensdauer zurück. Erfolgreiches Pflanzen, Fusionieren oder Heilen verbraucht ihn genau einmal. Ungültige Ziele und fehlgeschlagene Erzeugung behalten den Eimer. Pause und Spielgeschwindigkeit folgen der nativen Simulation.

Die 50-%-Regel für normale Bucketheads stammt aus der Benutzeranweisung. Der lokale Fusion-4.0.5.2-Zombie-Almanach beschreibt bei normalen Bucketheads (4/13) den Magnetentzug; ausdrückliche Todes-Drops gehören dort zu speziellen Eisenpflanzen-Zombies. Diese zusätzlichen Zombiearten werden hier nicht eingeführt. Die ursprüngliche Recherche und die neue Spielregel sind in `references/bucket-drops/behavior.json` getrennt dokumentiert.

Der Hook gehört an den vollständigen `Zombie.TakeDamage`-Ablauf. `TakeHelmDamage` entfernt den Helm, bevor derselbe Treffer noch den Körper beschädigen kann; ein Drop direkt in `DropHelm` würde deshalb auch bei tödlichem Überlauf entstehen. Board und `DataID` sichern die Übergangsbeobachtung gegen Pool-Wiederverwendung. Es wird kein allgemeiner Todes- oder Beutehook verwendet. Der native Board-Zufall bestimmt die 50-%-Chance.

Lose Eimer verwenden eigene, vorübergehende Darstellungsanker. Diese sind nicht an die Maus gebunden. Vor dem Speichern werden Auswahl und Darstellungen entfernt; innerhalb derselben Sitzung erscheinen nicht verbrauchte Drops danach mit ihrer restlichen Lebensdauer wieder. **Lose Drops werden nicht im Spielstand gespeichert und verschwinden beim Neuladen eines Boards.** Gepflanzte Buckets, Fusionen und native Magnetgegenstände bleiben wie bisher gespeichert. Board-Ende und Mod-Ende räumen die Drops auf. Rezepte, offene Liste, bereits vorhandene Pflanzen und LawnGlove-DLL bleiben unverändert.

Offline geprüft: 72 Szenarioassertionen einschließlich beider Zufallsausgänge, einmaliger Würfe, Helm-Teilschaden, tödlichem Überlauf, Instant-Kill-Flags, Magnetentzug ohne Doppel-Drop, Pool-Wiederverwendung, Aufnahme/Abbruch, fehlgeschlagener Platzierung, Verbrauch, Pause, Landung und Ablauf; bestehende Chomper-Regressionen; statische Rezept-/Assetprüfung und Werkzeugprüfungen; Release-Build. Kein Spielstart und keine Spieltests. Benutzerprüfung: gewöhnliche Erbsentreffer bis zum Helmverlust, ungefähre Drop-Häufigkeit über mehrere Bucketheads, keine Drops bei Instant-Kills, Magnetentzug mit genau einem nutzbaren Eimer, Bodenposition/Klickbereich auf Rasen und Dach, Blinken/Ablauf, Auswahl/Abbruch, Fusion/Heilung sowie Pause und Speichern/Laden.

Installation: `Mods/FusionMod.dll`, Version 0.1.39. Nachweis: `references/release-0.1.39.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.38.dll` nach `Mods/FusionMod.dll` kopieren.

## Änderung 0.1.38: Bucket, Buck-nut und Buckshooter

Bucket ist als eigenständige niedrige Pflanze mit 2000 Lebenspunkten verfügbar. Zum Beschaffen zuerst einen normalen Magnet-shroom einen Eimer von einem Buckethead anziehen lassen. Den Magnet-shroom mit dem aufgenommenen Eimer anklicken; am Handschuhcursor erscheint die Originalgrafik des Eimers. Ein freies gültiges Feld pflanzt Bucket. Ein Peashooter oder eine Wall-nut erzeugt unmittelbar die jeweilige Fusion. Rechtsklick oder Werkzeugwechsel hebt die Auswahl auf. Der gespeicherte Magnetgegenstand wird erst nach erfolgreicher Platzierung, Fusion oder Heilung verbraucht; ungültiges Terrain und fehlgeschlagene Erzeugung behalten ihn. Es gibt keine zusätzliche Samenkarte und keine Sonnenkosten für den aufgenommenen Eimer.

```text
Peashooter | Bucket | Buckshooter
Wall-nut | Bucket | Buck-nut
```

Beide Rezepte unterstützen auch gepflanzte Buckets: die native Zutatenkarte auf Bucket oder beide Handschuhrichtungen. Karten behalten ihre normalen Kosten und Cooldowns; der Handschuh behält seine bestehenden Regeln. Ergebnis entsteht vor Zutatenverbrauch. Unterlagen und Kürbisse bleiben erhalten. Passende Fusions- und Heilziele leuchten über den bestehenden Hervorhebungsweg.

Buck-nut hat 8000 Lebenspunkte und die drei Original-Schadensdarstellungen. Buckshooter hat 2000 Lebenspunkte und schießt alle 1,5 Sekunden eine originale Eisenerbse mit 80 Schaden. Zusätzlich verursacht der Treffer Schaden in Höhe der aktuellen zweiten Rüstung, in Replanted `mShieldHealth`; der Helm zählt nicht dazu. Überlebende bewegliche Bodenziele werden um 16 Board-Pixel zurückgestoßen. Das entspricht den belegten 0,2 Quell-Einheiten bei 80 Board-Pixeln pro Feld. Bosse, Fahrzeuge und Gargantuars werden nicht verschoben. Native Zielsuche, Flug, Trefferberechtigung und Treffereffekte bleiben verantwortlich. Torchwood verwandelt Eisenerbsen nicht in Feuererbsen.

Ein weiterer Eimer heilt Buck-nut und Buckshooter um 500 Lebenspunkte, höchstens bis zur normalen Obergrenze. Bereits vorhandene überschüssige Gesundheit wird nicht vermindert. Dafür entweder den aufgenommenen Magnet-Eimer auf die Eisenpflanze setzen oder einen gepflanzten Bucket mit dem Handschuh hintragen. Die Eisenpflanze wird geheilt und behält Identität und Schusszustand. Bucket selbst ist keine Eisenfusion und nimmt keine solche Heilung an. Dünger heilt weiterhin alle drei Pflanzen über den bestehenden Weg und lässt ihre Identität bestehen.

### Originalassets und Quellen

Alle Grafiken, Hierarchien, Pivots, Spritereihenfolgen und Originalclips kommen aus dem lokalen `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`. Fremde Unity-Spielskripte werden nicht ausgeführt.

- Bucket: `BucketPlantPrefab` 40920, Transform 116194, Controller 6196; fünf Knoten, vier Sprites, Originalclip `idle` 3897. Beschädigte Körper werden entsprechend der Gesundheit eingeblendet.
- Buck-nut: `IronNutPrefab` 64886, Transform 98626, Controller 6582; sieben Knoten, vier Sprites, Clips `idle` und `nothing` 4880/4882. Die Quelle verwendet hier ausdrücklich das gemeinsame Script `WallNut`, keine Klasse `IronNut`.
- Buckshooter: `IronPeaPrefab` 37176, Transform 120494, Controller 6613; 16 Knoten, 15 Sprites, Clips `idle`/`shoot` 4950/4951. Die additive Schussebene bleibt erhalten.
- Eisenerbse: `Bullet_ironPea` 33960, Transform 117733; zwei Knoten und zwei Originalsprites. Der Quellprefab hat keinen Animator.

Belege liegen unter `references/bucket/`, `references/buck-nut/`, `references/buckshooter/` und `references/iron-projectile/`, einschließlich tatsächlicher Methodenadressen, Original-Almanach, komprimierter Enum-Konstanten, Prefab-/Scriptzuordnung, Exportprüfsummen und Offlinevorschauen. Die Rezepte sind mit Registrierungen `0x9d5846` und `0x9d591a` belegt. Die Quellen-ID 224 bezeichnet die Grundzutat und hat selbst kein Fusionsrezept. Der Recherchehelfer unterstützt dafür jetzt `--source-id` und prüft Name und ID gegen das unveränderte Rezeptarchiv.

### Speicherung und Portierungsgrenzen

Gespeicherte Paare `mSeedType + mImitaterType`: Bucket `Potatomine + Magnetshroom`, Buck-nut `Wallnut + GoldMagnet`, Buckshooter `Peashooter + Magnetshroom`. Die Marker sind Identitäten, keine zusätzlichen Zutaten. Der Potato-Mine-Typ liefert nur die niedrige native Ziel-/Terrainkategorie; sein Minenverhalten ist für Bucket abgeschaltet. Identität, Gesundheit und Schusstakt werden nativ gespeichert. Die Auswahl eines Magnet-Eimers wird vor dem Speichern aufgehoben; der noch nicht verbrauchte Gegenstand bleibt beim Magnet-shroom gespeichert. Tod, Pool-Wiederverwendung, Boardwechsel und Mod-Ende bereinigen Auswahl und Projektilherkunft.

Die Beschaffung verwendet den nativen Replanted-Magnetablauf samt Anziehung, Aufladung und Gegenstandsdauer. Die frei herumliegenden Fusion-Gegenstände mit eigener Flugphysik, Blinken nach vier Sekunden und Verschwinden nach acht Sekunden werden nicht zusätzlich nachgebaut. Andere Metallgegenstände bleiben im nativen Magnetablauf; sie werden in diesem Durchlauf nicht zu neuen Zutaten. Eisenpflanzen können mit den implementierten Eimern geheilt werden.

Die Quelle halbiert feindlichen Kirsch-Explosionsschaden an Eisenfusionen. Replanted hat keine feindlichen Kirsch-Explosionen; eigene Cherry Bombs und bisherige Mod-Kirsch-Explosionen beschädigen Pflanzen überhaupt nicht. Diese native Regel bleibt erhalten. Jack-in-the-Box und Zerquetschen sind andere Schadensarten und werden für diese drei Pflanzen nicht abgeschwächt. Das ist keine allgemeine Explosionsimmunität.

Die Herkunft fliegender Eisenerbsen bleibt wie bei bisherigen Mod-Geschossen flüchtig: Nach Speichern/Laden werden bereits fliegende Geschosse native Erbsen. Neue Schüsse funktionieren wieder als Eisenerbsen. Audio und Trefferpartikel stammen aus Replanted. Spielstände mit den neuen Pflanzen benötigen diese oder eine kompatible neuere Mod-Version.

Offline geprüft: 49 Szenarioassertionen zu Rezepten, Speicherung der Identität, Werten, Auswahl, Abbruch, Fehlerfällen, Heilung, Handschuh, Projektilherkunft, Schildschaden und Rückstoß; bestehende Chomper-Prüfungen; Werkzeugprüfungen; statischer Abschluss und Release-Build. Kein Spielstart und keine Spieltests. Benutzerprüfung: Eimeraufnahme und Cursor, alle Rezeptreihenfolgen mit Karten/Handschuh, Kosten, Unterlagen/Kürbisse, Dach/Wasser, Schadensbilder, Schildtreffer und Rückstoß, Heilung, Pause, Speichern/Laden, Boardwechsel und unveränderte normale Pflanzen.

Die zwei erledigten Ergebniszeilen sind aus `fusionen.txt` entfernt. Rezepte mit Bucket/Buckshooter/Buck-nut für weitere Ergebnisse bleiben offen. Aktueller Stand: 41 aktive Rezepte, 627 offene Rezepte für 523 Ergebnisse. Quellenarchiv unverändert.

Installation: `Mods/FusionMod.dll`, Version 0.1.38. Nachweis: `references/release-0.1.38.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.37.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.

## Änderung 0.1.37: Heilung vor dem Düngen, Wachstum danach

Ein erfolgreicher Düngereinsatz heilt jetzt zunächst alle lebenden Pflanzen der gewählten Zelle und aktualisiert deren Schadensdarstellung. Erst danach beginnt die native Dünganimation. Wall-nut, freie Lily Pad und Spikeweed werden erst nach dem tatsächlichen Ende dieser Animation ersetzt. Auch Sun-shrooms beginnen ihr Wachstum erst danach. Es gibt keinen geschätzten Animationstimer und keine zweite Heilung nach der Animation. Schäden während des Düngens bleiben möglich; normale Ersatzerzeugung liefert weiterhin die volle Gesundheit der neuen Pflanze.

Die ausstehende Wachstumsoperation verwendet den bereits vorhandenen nativen `GridItemType.ZenTool` mit `ZenToolFertilizer`, Zielfeld, erwarteter Pflanzenart und nativer Reanimations-ID. Diese Daten werden regulär vom Spiel gespeichert und geladen. Ein Patch am tatsächlichen Aufrufpfad `Board.UpdateGridItems -> GridItem.Update` übernimmt ausschließlich Düngerwerkzeuge außerhalb des Zen Gardens. Er wartet auf das native Animationsende, prüft Pflanze, Belegung und Terrain erneut und führt das Wachstum aus. Der Zen-Garden-Inventar- und Profilpfad wird dabei nicht aufgerufen. Leere Zellen, entfernte oder getragene Pflanzen, Fusionsvarianten und inzwischen belegte Seerosen werden nicht umgewandelt. Bei fehlgeschlagener Ersatzerzeugung bleibt die ursprüngliche Pflanze erhalten. Nach Ende wird das native Werkzeug aufgeräumt. Die bisher dokumentierte Ladegrenze der unbenutzten Drop-Beutel gilt weiterhin; bereits gestartetes Wachstum bleibt jetzt nativ gespeichert.

Der Drop-Beutel verwendet die Originalskalierung seines nativen Animationscontrollers. Der bisherige zusätzliche Faktor `0.45` entfällt. Der Mausbereich wird passend zum größeren Beutel erweitert. Cursor-Größe und Dünganimation bleiben unverändert.

Quellenbelege unter `references/fertilizer/`: native `GridItem.Init`, `GridItem.Update`, `GridItem.Serialize`, `GridItem.Deserialize`, `GridItem.GridItemDie` und der direkte Aufruf in `Board.UpdateGridItems`; zusätzlich Originaldaten des Dünger-Cursors. Die Serialisierung enthält sowohl die erwartete Pflanzenart als auch die Reanimations-ID. Release-Build und statische Ressourcen-/Rezeptprüfung erfolgreich. Kein Spielstart und keine Spieltests. Der Benutzer prüft sichtbare Sofortheilung, Wachstum erst nach der Dünganimation, größere Drops samt Klickbereich, Pause/Spielgeschwindigkeit sowie Speichern/Laden während der Animation.

Installation: `Mods/FusionMod.dll`, Version 0.1.37. Nachweis: `references/release-0.1.37.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.36.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.

## Änderung 0.1.36: Dünger im Kampf

Am Haus fällt regelmäßig ein Düngerbeutel herunter. Der erste Drop erfolgt nach 70,5 Simulationssekunden, weitere Drops alle 72 Sekunden. Das entspricht dem tatsächlichen Fusion-4.0.5.2-Controller `Bg1` mit seinem acht Sekunden langen Clip `motion1`, dem Ereignis bei 6,5 Sekunden und `GiveFertilize.AnimGive` nach jeweils neun Ereignissen. Ein unaufgesammelter Beutel verschwindet nach 30 Sekunden; während der letzten fünf Sekunden blinkt er. Die 30 Sekunden sind direkt aus der tatsächlichen `Fertilize.Update`-Methode und ihrer Float-Konstante belegt. Die zusätzliche Blinkdarstellung erfüllt die gewünschte Sonnen-Anmutung. Pause und Spielgeschwindigkeit folgen der nativen 100-Hz-Simulation.

Bedienung: Beutel links am Haus anklicken, danach eine belegte Pflanzenzelle anklicken. Ungültige Ziele behalten den Dünger am Cursor. Rechtsklick oder Werkzeugwechsel legt den Beutel mit seiner übrigen Lebensdauer zurück. Die Zeit läuft während des Tragens nicht ab. Ein erfolgreicher Einsatz verbraucht genau einen Beutel und spielt die native Zen-Garden-Dünganimation. Grafik und Cursor stammen aus Replanted; fremde Fusion-Spielskripte werden nicht ausgeführt. Die Darstellung am Haus verwendet den eingefrorenen Anfang dieser Animation und eine kurze Fallbewegung. Alle veränderten Eigenschaften des gepoolten Animationscontrollers werden vor seiner Rückgabe wiederhergestellt.

Der Dünger heilt sämtliche lebenden Pflanzen der gewählten Zelle vollständig, einschließlich Fusionen, Unterlagen und Kürbissen. Bereits vorhandene Gesundheit oberhalb des normalen Maximums bleibt erhalten. Normale Wall-nuts wachsen unabhängig vom vorherigen Schaden zu Tall-nuts. Freie Lily Pads wachsen zu Cattails; bei einer belegten Seerose bleiben Unterlage und vorhandene Pflanze erhalten und werden geheilt. Spikeweed wächst zu Spikerock. Kleine oder bereits wachsende Sun-shrooms beginnen sofort die native Wachstumsanimation. Voll ausgewachsene Sun-shrooms werden geheilt. Fusionsvarianten werden über ihre gemeinsame Identitätsauflösung erkannt und behalten ihre Identität. Ersatzpflanzen werden vor dem Entfernen der ursprünglichen Pflanze erstellt. Terrainprüfungen, Unterlagen und Kürbisse bleiben berücksichtigt; Kosten, Karten und Cooldowns werden nicht verbraucht.

Dünger ist auf laufenden Spielbrettern verfügbar, nicht im Zen Garden. Die Fusion-Fortschrittssperre und weitere nicht beauftragte Fusion-Sonderupgrades werden nicht übernommen. Es wird weder Zen-Garden-Inventar verbraucht noch ein Profil verändert. Beim Speichern wird die Auswahl aufgehoben; laufende Beutel und Timer bleiben während der Sitzung erhalten. Ihre nur vorübergehenden Darstellungen werden vor der nativen Serialisierung entfernt und anschließend neu aufgebaut. Beim Laden eines neuen Boards beginnen Drop-Timer und Beutelbestand neu. Die Pflanzenheilung, Ersatzpflanzen und Sun-shroom-Zustände verwenden native gespeicherte Pflanzenfelder.

Quellenbelege: `references/fertilizer/behavior.json`, tatsächliche Fusion-Methoden, Originalprefab und Originalclip sowie native Replanted-Methoden. Release-Build und statische Ressourcen-/Rezeptprüfung erfolgreich. Kein Spielstart, keine Spieltests. Der Benutzer prüft Drop und Mausposition, 72-Sekunden-Takt, Ablauf/Blinken, Auswahl/Abbruch, Heilung und alle vier Wachstumswege, beschädigte Nüsse, belegte Seerosen, Kürbisse, Handschuh, Pause/Spielgeschwindigkeit sowie Speichern/Laden.

Installation: `Mods/FusionMod.dll`, Version 0.1.36. Nachweis: `references/release-0.1.36.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.35.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert.

Zielspiel: `/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted/`

Arbeitsverzeichnis: `/home/kevin/modding/pvzreplanted/fusion-mod/`

## Änderung 0.1.35: Pea-nut-Fallbewegung und richtige Chompzilla-Grafik

Pea-nut wirft die Rollnuss jetzt aus Mundhöhe ab. Bei einer normalen 80-Pixel-Pflanze beginnt sie 45 Board-Pixel über ihrer Bodenposition, fällt mit 0,1 Pixel pro Tick zusätzlicher Fallgeschwindigkeit und landet ohne Sprung oder Einsinken. Während des Falls bewegt sie sich mit 2,5 Pixeln pro Tick nach vorn; nach der Landung rollt sie mit 1,25 statt bisher 3,33 Pixeln pro Tick durch die Reihe. Der Originalschatten bleibt während des Falls auf dem Boden. Native `mPosZ`/`mVelZ`/`mAccZ` beschreiben die Höhe; `Projectile.Update` und der Controller behalten die Positionierung. Schaden, Durchdringung, Zielberechtigungen und Rollnussgröße bleiben erhalten. Die Rollnuss verschwindet erst im Bodenkontakt an einer ansteigenden Terrainkante. Chomp-shooter- und Chompzilla-Brocken behalten ihr bisheriges Bewegungsverhalten.

Die Grafik von Chompzilla war in 0.1.34 falsch zugeordnet. `UltimateChomper` bezeichnet in dieser Quelle Cherrizilla (ID 903); die beauftragte Chompzilla hat ID 1013 und heißt intern `SuperChomper`. Die beiden Werte sind aus den komprimierten Enum-Konstanten der tatsächlichen 4.0.5.2-Metadaten belegt. Der Import verwendet jetzt `SuperChomperPrefab` 56150, Transform 111852, Script 194393 (`globalgamemanagers.assets:163 SuperChomper`), Animator 126182 und Controller 6640. Alle 32 Knoten, 28 Sprites und die beiden Originalclips 5031 `idle` und 5030 `bite` sind eingebettet. Kopf und Körper wechseln zwischen den drei Original-Schadensstufen. Die bisher gewählte Cherrizilla-Grafik und deren zusätzlicher Clip sind nicht mehr im aktiven Modell enthalten. Verwaiste Originalkurven sind gezielt anhand des richtigen Prefabs belegt; alte unbenutzte Spritedateien werden nach einem erfolgreichen Export entfernt.

Rezepte, gespeicherte Pflanzenkennungen und Chompzillas vorhandene Fähigkeiten ändern sich nicht. Die Beschreibung der in 0.1.34 verwendeten Cherrizilla-Assets bleibt unten als historischer Fehlernachweis stehen. Aktuelle Identitäts-, Prefab-, Controller- und Methodenbelege stehen in `references/chompzilla-0-1-35/`; das aktuelle Exportmanifest liegt weiterhin unter `references/chompzilla/visual-assets.json`. Der bisherige Manifeststand ist als `visual-assets-0.1.34.json` erhalten. Ergänzende Rollnussquellen liegen unter `references/pea-nut-0-1-35/`.

Offline geprüft: 57 Szenarioassertionen einschließlich Fallbewegung, Landung, langsamerem Rollen und unverändertem Chomp-shooter; neun Werkzeugprüfungen einschließlich des Abgleichs von Quellen-ID, Script und importiertem Prefab; statischer Ressourcen-/Rezeptabschluss; Release-Build. Die Assetvorschau zeigt die korrigierte Originalgrafik. Kein Spielstart und keine Spieltests. Der Benutzer prüft Abwurf, Schatten und Landung auf Rasen/Dach, langsameres Rollen, mehrere Treffer, Pause/Spielgeschwindigkeit, Chompzillas Aussehen und Schadensstufen sowie unveränderte Brocken der beiden Chomper-Fusionen. Die bekannte Speichergrenze fliegender Mod-Geschosse bleibt bestehen.

Installation: `Mods/FusionMod.dll`, Version 0.1.35. Nachweis: `references/release-0.1.35.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.34.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände, Profile und installierte LawnGlove-DLL bleiben unverändert.

## Änderung 0.1.34: Pea-nut, Chomp-shooter, Chomp-nut und Chompzilla

Alle sechs Quellenrezepte sind implementiert. Die drei Zweierfusionen werden zuerst verfügbar; danach sind alle drei Wege zur Chompzilla mit bereits vollständig implementierten Zutaten möglich. Karten und Handschuh unterstützen beide Zutatenreihenfolgen über die gemeinsamen Platzierungs-, Kosten-, Hervorhebungs- und Verbrauchswege:

```text
Peashooter | Wall-nut | Pea-nut
Chomper | Peashooter | Chomp-shooter
Chomper | Wall-nut | Chomp-nut
Chomp-nut | Peashooter | Chompzilla
Chomp-shooter | Wall-nut | Chompzilla
Chomper | Pea-nut | Chompzilla
```

**Pea-nut** hat 4000 Lebenspunkte. Native Peashooter-Zielsuche und Schussauslösung bleiben erhalten. Alle 1,5 Sekunden entsteht eine rollende Nuss mit 20 Schaden. Sie trifft jeden berechtigten Bodenzombie ihrer Reihe einmal und durchdringt beliebig viele Ziele. Rissstufen verwenden die Originalkörper. **Chomp-shooter** nutzt natives Verschlucken und native Einschränkungen für große Ziele. Die Verdauung dauert 30 Sekunden. Nur während dieser Verdauung spuckt er alle drei Sekunden einen rollenden Brocken mit 80 Schaden und derselben Durchdringung. Der erste Brocken entsteht nach etwa 0,83 Sekunden am Ereignis des Originalclips. **Chomp-nut** hat 4000 Lebenspunkte, verschluckt nativ, verdaut 40 Sekunden und heilt am Ende der Verdauung 1000 Lebenspunkte bis höchstens 4000. Beide Original-Rissstufen für Kopf und Rücken werden bei Schaden und Heilung neu gewählt.

**Chompzilla** hat 4000 Grund-Lebenspunkte und beißt im Nahbereich alle 1,75 Sekunden. Der Biss trifft am entsprechenden Zeitpunkt des Originalclips; ein inzwischen verschwundenes Ziel ergibt einen Fehlbiss. Der bereite Spezialbiss verursacht 1.000.000 Schaden, heilt bis zur Obergrenze 16000 und erhält anschließend 40 Sekunden Abklingzeit. Während dieser Zeit beißt Chompzilla weiter mit 200 Schaden und heilt um den verursachbaren Schaden. Ziele ohne verbleibenden Helm und mit höchstens 500 Körper-Lebenspunkten erhalten stattdessen 500 Schaden und liefern 400 Heilung. Jeder erfolgreiche Biss spuckt einen durchdringenden Brocken mit 80 Schaden. Der Brocken des verstärkten normalen Bisses stößt lebende, bewegliche Bodenziele um 40 Board-Pixel zurück. Bosse, Fahrzeuge und Gargantuars werden nicht verschoben. Lebenspunkte oberhalb 4000 sinken pro Simulationssekunde um 200; die Obergrenze beträgt 16000.

Die Schadensbegrenzung von Chompzilla setzt native Zerquetschung sowie die Pflanzenexplosion des Jack-in-the-Box in höchstens `4000 / 3 = 1333` Schaden pro Treffer um. Niedrige Gesundheit kann weiterhin zum Tod führen. Gewöhnliche numerische Zombie- und Katapulttreffer liegen bereits unter dieser Grenze. Der native Radiushelfer bestimmt die Explosionsempfänger; nur dessen synchroner `Plant.Die`-Pfad wird umgedeutet. Schaufel, Fusionsverbrauch, Bungee und Board-Abbau bleiben echte Entfernungen. Die tatsächlichen Aufrufer sind statisch belegt: `SquishAllInSquare` ruft `Plant.Squish`; Jack-in-the-Box ruft den Float-Radiushelfer, der `Plant.Die` aufruft. Native Zähler für zerquetschte/verlorene Pflanzen können dabei trotz überlebender Chompzilla erhöht werden; dieser Statistikunterschied betrifft nicht Gesundheit oder Rezepte.

### Originalassets, Speicherung und Grenzen

Grafik, Hierarchien, Pivots, Spritereihenfolge und sämtliche Clips stammen aus dem lokalen 4.0.5.2-Bundle. Es werden keine fremden Spielskripte oder Collider ausgeführt:

- Pea-nut: `NutShooterPrefab` 41474, Transform 116621, Controller 6618; zwei Clips, 15 Knoten, acht Sprites.
- Chomp-shooter: `PeaChomperPrefab` 65657, Transform 92526, Controller 6619; drei Clips, 26 Knoten, 24 Sprites. Der Shoot-Clip bleibt additiv zur Basisebene.
- Chomp-nut: `NutChomperPrefab` 45123, Transform 94296, Controller 6616; vier Clips, 20 Knoten, 23 Sprites einschließlich vier separat geladenen Rissgrafiken. Deren ursprüngliche ResourceManager-Pfade lauten `plants/_mixer/nutchomper/cracked1_back`, `cracked2_back`, `cracked1_head`, `cracked2_head`.
- Chompzilla: `UltimateChomperPrefab` 56149, Transform 111851, Controller 6638; drei Clips, 42 Knoten, 29 Sprites. Der normale Biss läuft über `bite`. Der zusätzliche Originalclip `supershoot` ist mit exportiert, gehört jedoch zum zusätzlichen Quell-Spezialangriff und wird für die normale Rezeptpflanze nicht ausgelöst.
- Rollnuss: `Bullet_nut` 50823, Transform 116654. Brocken: `Bullet_pea_chomper` 33967, Transform 117740. Beide Originalhierarchien mit jeweils drei Knoten und zwei Sprites sind eingebettet. Die Bewegung dreht den sichtbaren Körper, der Schatten bleibt unten. Die Rollbewegung verwendet 3,33 Board-Pixel pro Simulationstick. An einer ansteigenden Terrainkante verschwinden die Geschosse. Sie werden nicht durch Torchwood zu Feuererbsen; Schilde verhindern ihre Durchdringung nicht.

Wie beim bisherigen Import bilden Replanted-Zustände und Simulationstakte die Originalanimationen ab; deren Events erzeugen keine zusätzlichen Angriffe. Chompzilla verwendet für den fehlenden nativen wiederholten Nahbiss einen eigenen Ablauf am tatsächlichen `UpdateAbilities`-Besitzer. Native Chomper-Zielsuche bestimmt weiterhin Reichweite und Zielberechtigungen. Quellenoptionen für Reise-Upgrades, legendäre Fähigkeiten und manuell aktivierte zusätzliche Angriffe sind nicht Bestandteil dieser normalen Rezeptpflanzen. Importiert werden Pflanzen und Geschosse; Audio und Partikel bleiben Replanted zugeordnet. Die neuen Rollgeschosse haben keinen zusätzlichen eigenen Quellton oder Trefferpartikel.

Reservierte gespeicherte Paare (`mSeedType + mImitaterType`): Pea-nut `Peashooter + Wallnut`, Chomp-shooter `Chomper + Peashooter`, Chomp-nut `Chomper + Wallnut`, Chompzilla `Chomper + Tallnut`. Der Tallnut-Marker identifiziert Chompzilla und ist keine weitere Zutat. Gesundheit, Identität, Bissphase und Spezial-/Verdauungszeit liegen in nativen gespeicherten Feldern. Originaldarstellung wird nach Laden wieder aufgebaut. Es gibt keinen zusätzlichen Pflanzentimer in einer flüchtigen Liste. Die Herkunft und bereits getroffene Ziele fliegender Rollgeschosse sind dagegen flüchtig: Beim Laden werden vorhandene Geschosse als native Erbsen rekonstruiert. Spielstände daher vorzugsweise ohne fliegende Mod-Geschosse speichern; diese Grenze entspricht der bisherigen Herkunftsverwaltung anderer Mod-Geschosse. Tod, Pool-Reset, Board-Ende und Mod-Ende räumen die Laufzeitverwaltung auf.

Belege: `references/pea-nut/`, `references/chomp-shooter/`, `references/chomp-nut/`, `references/chompzilla/`, `references/nut-projectile/` und `references/chomp-projectile/`. Sie enthalten alle Quellenrezepte mit Registrierungsadressen, Original-Almanach, tatsächliche Methoden, Prefabs, Controller, Exportprüfsummen und Offlinevorschauen. Verwaiste Transformkurven der Originalclips sind mit vollständiger Prefab-Pfadliste belegt; nur diese bekannten Kurven werden übersprungen. Das Quellenarchiv bleibt unverändert. Die sechs erledigten Ergebniszeilen sind aus `fusionen.txt` entfernt; Rezepte mit diesen Pflanzen als Zutaten für noch offene Ergebnisse bleiben erhalten. Stand: 39 aktive Rezepte, 629 offene Rezepte für 525 Ergebnisse.

Offline geprüft: 51 Szenarioassertionen gegen die echten Rezept-, Identitäts-, Fähigkeits- und Rollgeschossdateien mit Testdoubles, vorhandene Cherry-Chomper-Regression, acht Werkzeugprüfungen, vollständiger statischer Abschluss und Release-Build. Das sind keine Spieltests. Spielstart und Laufzeitprüfung übernimmt ausschließlich der Benutzer. Spielstände und Profile werden während der Arbeit nicht geöffnet oder geändert.

Manuell prüfen: alle sechs Rezepte in beiden Karten-/Handschuhrichtungen, Kosten/Cooldown, Abbruch, Terrain und erhaltene Unterlagen/Kürbisse; Originalgrafik einschließlich Vorschauen und Rissstufen; Rollgeschosse gegen mehrere Zombies, Schilde, Flugziele, Verbündete, Dach und ansteigendes Terrain; 30-/40-Sekunden-Verdauung, Brocken ausschließlich während Verdauung, Heilungsgrenzen; Chompzilla gegen große Ziele, 40-Sekunden-Spezialabstand bei fortgesetzten normalen Bissen, schwache Ziele, Rückstoß, überschüssige Gesundheit, Gargantuar/Zamboni/Jack-in-the-Box; Pause, Spielgeschwindigkeit, Tragen, Bungee, Tod, Boardwechsel sowie Speichern/Laden während Verdauung und Bissphase. Gewöhnliche Chomper/Peashooter und bisherige Fusionen gegenprüfen.

Installation: `Mods/FusionMod.dll`, Version 0.1.34. Nachweis: `references/release-0.1.34.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.33.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit den neuen Varianten benötigen 0.1.34 oder eine kompatible neuere Version. LawnGlove bleibt unverändert.

## Änderung 0.1.33: Cherry-Bomb-Fusionsexplosion und Größenanpassungen

Bei jeder erfolgreichen Fusion mit einer normalen Cherry Bomb als Zutat entsteht einmal eine normale Kirsch-Explosion auf dem Zielfeld. Das gilt für beide Kartenreihenfolgen und beide Handschuhrichtungen, auch für die Fusion zum Explode-o-shooter. Erst entsteht das Ergebnis, dann werden die Zutaten verbraucht, anschließend explodiert das Zielfeld. Abbruch, ungültige Platzierung, Vorschau, Laden und geskriptete Spawns erzeugen keine Fusionsexplosion. Kirschvarianten ohne neue Cherry Bomb als Zutat lösen sie ebenfalls nicht aus.

Die Explosion verwendet den nativen Schadenshelfer mit denselben Parametern wie Cherry Bombs `Plant.DoSpecial`: Radius 115 Board-Pixel, höchstens eine Nachbarreihe, Brandschaden und native Zielkategorien `0x7f`. Dazu kommen der normale Powie-Effekt, beide nativen Explosionstöne, Board-Erschütterung und die vorhandene Cherry-nut-Heilung. Der Ergebnispflanze bleibt ihr eigenes Verhalten erhalten; insbesondere behält Solar Bomb ihren nachfolgenden Zünder. Zutatenverbrauch löst weiterhin keine zusätzlichen Todeseffekte aus.

Die grafische Explosion des Explode-o-shooter-Projektils ist 10 % kleiner. Nur die erzeugte Partikelinstanz wird skaliert; 300 Schaden und Radius 80 bleiben erhalten. `TodParticleSystem.OverrideScale` skaliert nativ den zugehörigen Effektcontroller, ohne die gemeinsame Partikeldefinition zu ändern.

Giant Wall-nut ist 10 % kleiner: Skalierung 1,8 statt 2,0 und logische Größe 144 statt 160 Board-Pixel. Das Rolltempo sinkt unabhängig von der Darstellung um 10 %: Faktor 0,45 statt 0,5. Kontaktregeln, 1800 Schaden und Rollanimation bleiben erhalten.

Nur Build und statische Prüfungen; kein Spielstart und keine Spieltests. Die Laufzeitprüfung übernimmt der Benutzer: Karten und Handschuh jeweils in beiden Richtungen, Explosion ausschließlich auf dem Zielfeld, fehlende Explosion bei Abbruch, Schaden und Cherry-nut-Heilung, kleinerer Projekteffekt sowie Größe und Rolltempo der Giant Wall-nut. Release-/Installationsnachweis: `references/release-0.1.33.json`. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.32.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände, Profile und LawnGlove bleiben unverändert.

## Änderung 0.1.32: Cherry-Chomper-Spezialfähigkeit im tatsächlichen Spielpfad

Beim erfolgreichen Verschlucken erzeugt Cherry Chomper wie in PvZ Fusion eine Kirsch-Explosion ein Feld voraus: 1800 Schaden, Radius 160 Board-Pixel mit höchstens einer Nachbarreihe. Der bestehende gemeinsame Explosionsweg übernimmt Zielberechtigungen, Schild-/Körperschaden, Partikel, Ton und Cherry-nut-Heilung. Native Zielsuche, Fehlbisse, Bisse auf große Ziele und 40 Sekunden Verdauung bleiben erhalten.

Der Benutzer meldete für 0.1.31, dass die Spezialfähigkeit fehlt. Der bisherige Patch hing an `Plant.UpdateChomper`. Die tatsächliche Replanted-DLL hat dessen Logik jedoch zusätzlich direkt in `Plant.UpdateAbilities` einkompiliert: `Plant.Update` ruft `UpdateAbilities` auf; dieses enthält den erfolgreichen Schluckvorgang und ruft `UpdateChomper` nicht auf. Dadurch erreichte normales Spielen den bisherigen Patch nicht. Das war ein Fehler der ersten Portierung.

Der Patch erfasst jetzt den nativen Zustand vor und nach `UpdateAbilities`. Nur der erfolgreiche Übergang `ChomperBiting` nach `ChomperBitingGotOne` erzeugt einmal die Explosion. Der native Besitzer der Fähigkeit bleibt erhalten; es gibt keinen zusätzlichen Timer und keine globale Schluckregistrierung. Verdauung, Laden eines bereits erreichten Schluckzustands, Fehlbisse, gewöhnliche Chomper, Vorschauen und Board-Abbau erzeugen keine zusätzliche Explosion. Rezepte, Originalgrafik, Animationen und Speicherkennung ändern sich nicht. Die erledigte Ergebniszeile bleibt aus `fusionen.txt` entfernt.

Statischer Nachweis: `references/cherry-chomper/replanted-Update.asm`, `replanted-UpdateAbilities.asm` und `fix-0.1.32.json`. Die tatsächliche Quelle `CherryChomper.Chomp` bei RVA `0x475f80` und der lokale Almanach belegen weiterhin die Fähigkeit. Offline-Regressionsprüfung kompiliert die echte Patchdatei mit kleinen Testdoubles, prüft den verantwortlichen Patchpunkt, erfolgreichen Schluckvorgang, Einmaligkeit, Explosionsposition und ausgeschlossene Fälle. Sie lädt keinen Spielcode. Werkzeugprüfungen und statischer Abschluss bleiben zusätzlich erforderlich.

Release-/Installationsnachweis: `references/release-0.1.32.json`. Vorversion: `backup/FusionMod-0.1.31.dll`. Kein Spielstart und keine Änderungen an Profilen oder Spielständen. Die Laufzeitbestätigung bleibt beim Benutzer: einen Zombie verschlucken lassen und Explosion samt Flächenschaden auf weitere Zombies prüfen; Fehlbiss, große Ziele, Verdauung sowie Speichern/Laden gegenprüfen.

## Änderung 0.1.31: Automatisierter Workflow und Cherry Chomper

`tools/source_index.py` erfasst Prefabs/GameObjects, Klassen, MonoScripts, Animatorcontroller und Clips einmal. Der JSON-Cache unter `.cache/source-index.json` wird vor jeder Nutzung anhand von SHA-256 für Bundle, DLL, Metadaten und Indexskript geprüft. Geänderte Quellen verwerfen den Cache; die DLL muss weiterhin zum vollständigen Quellenarchiv passen. Die Recherche verwendet den Klassenindex. Der Exporter akzeptiert wiederholtes `--plant` und `--all`; pro Aufruf wird das Bundle genau einmal geladen.

`tools/fusion_checks.py recipes` zeigt alle Quellenrezepte oder sämtliche Alternativen eines Ergebnisses. `tools/ingredient-availability.json` erfasst geprüfte Verfügbarkeit mit Replanted-Identität und Begründung ausdrücklich. Nicht erfasste oder als nicht verfügbar markierte Zutaten sperren ein Rezept; eine Registrierung schaltet keine Zutat frei. `--strict` meldet gesperrte Alternativen zusätzlich durch einen Fehlerstatus. Die statische Abschlussprüfung prüft Speicherkennungen, Rezept-/Verfügbarkeitsabgleich, Ressourcen und Exportprüfsummen, aktuelle Versionsangaben sowie die vollständige offene Liste und deren Zahlen. Historische Versions- und Zahlenangaben bleiben historische Belege.

`tools/release.py` erhöht standardmäßig die Patchversion gemeinsam, prüft, baut Release, sichert die installierte Vorversion ohne Überschreiben einer abweichenden Sicherung und ersetzt die DLL atomar. Binärgleichheit und DLL-Dateiversion werden geprüft; `SHA256SUMS` und ein Releasebeleg werden aktualisiert. Eine exklusive Installationssperre verhindert parallele Releases. Bei einem Fehler nach Installation werden DLL, Prüfsummen und bearbeitete Versionsdateien wiederhergestellt. LawnGlove wird nicht installiert oder verändert. Einzelheiten und reproduzierbare Aufrufe stehen in `vorgehen-neue-fusionen.md`.

### Cherry Chomper als erster vollständiger Durchlauf

Das einzige Quellenrezept lautet `Cherry Bomb | Chomper | Cherry Chomper` (Ergebnis-ID 1016). Beide Zutaten sind ausdrücklich als native, verfügbare Pflanzen erfasst. Beide Kartenreihenfolgen und beide Handschuhrichtungen verwenden die bestehenden zentralen Platzierungs-, Kosten-, Hervorhebungs- und Verbrauchswege. Das Ergebnis entsteht vor dem Zutatenverbrauch; Seerosen, Töpfe und Kürbisse bleiben erhalten.

Der lokale Almanach verlangt sofortiges Verschlingen, 40 Sekunden Verdauung und eine Kirsch-Explosion ein Feld vor der Pflanze beim erfolgreichen Verschlucken. Die tatsächliche Quellenmethode `CherryChomper.Chomp` bei RVA `0x475f80` bestätigt diesen Explosionsweg. Der native Replanted-Chomper übernimmt Zielsuche, erlaubte Ziele, erfolglose Bisse, Bisse auf zu große Ziele und den gespeicherten Verdauungszähler von 4000 Simulationsticks. Ausschließlich der native Übergang `ChomperBiting` nach `ChomperBitingGotOne` erzeugt einmal die gemeinsame Kirsch-Explosion: 1800 Schaden, Mittelpunkt 80 Board-Pixel vor der Pflanze, Radius 160 Pixel, höchstens eine Nachbarreihe. Die Explosion verwendet native Zielberechtigungen und Trefferrechtecke, schädigt Schild und Körper und heilt Cherry-nuts um 200 bis zu deren Maximum. Fehlbisse, große nicht verschluckte Ziele, Verdauung, Tod und Laden lösen keine zusätzliche Explosion aus. Native Bissregeln und Zeitpunkte bleiben erhalten; die Originalanimation erzeugt keine zusätzlichen Angriffsereignisse.

Originalmodell: `CherryChomperPrefab` 47417, Transform 120633, Script `globalgamemanagers.assets:2779 CherryChomper`, Animator 126815, Controller 6590. Alle 24 Knoten, 22 Sprites und vier Originalclips sind eingebettet: 4900 `idle`, 4898 `bite`, 4899 `chew`, 4901 `swallow`. Der Controller hat eine ersetzende Basisebene ohne Maske; der Renderer verbindet die Clips mit nativem Zustand und Animationsfortschritt. Die Quelldatei enthält 18 Transformbindungen auf sechs im Prefab fehlende Pfade in `bite`/`chew`. Nur diese exakt belegten Bindungen werden wie in Unity übersprungen und im Exportmanifest aufgeführt; unbekannte Bindungen bleiben Fehler. Belege einschließlich vollständiger Prefab-Pfadliste liegen unter `references/cherry-chomper/`. Explosionspartikel und Audio bleiben native Replanted-Assets.

Speicherkennung: `Chomper + Cherrybomb` in `mImitaterType`. Native Felder speichern Identität, Zustand und Verdauungszeit; beim Laden wird die Originaldarstellung wiederhergestellt. Keine neue flüchtige Zustandsliste und kein eigener Echtzeittimer. Die Ergebniszeile wurde nach Implementierung aus `fusionen.txt` entfernt. Cherry Chomper als Zutat für Cherrizilla bleibt offen. Aktuell: 33 aktive Rezepte, 635 offene Rezepte für 529 Ergebnisse. `fusionen-quellen.json` bleibt unverändert.

Offline geprüft: Cachewiederverwendung und Prüfsummeninvalidierung, Sammelexport von Cherry Chomper und bestehendem Kirschprojektil, vollständige Rezeptprüfung, statische Abschlussprüfung sowie acht Werkzeugprüfungen einschließlich Fehlererkennung und Release-Rückbau. Die Originalanimation wird zusätzlich als Offlinevorschau erzeugt. Release-/Installationsnachweis: `references/release-0.1.31.json`. Das sind keine Spieltests. Spielstart, Spielstände und Profile bleiben unangetastet.

Manuell durch den Benutzer prüfen: beide Karten-/Handschuhreihenfolgen innerhalb des Cherry-Bomb-Zündfensters, Kosten/Cooldown, Abbruch, gültige/ungültige Markierung, Unterlagen/Kürbis, erfolgreiches Verschlucken mit genau einer Explosion ein Feld voraus, Randfelder, Rüstung, Cherry-nut-Heilung, fehlende Explosion bei Fehlbiss und großen Zielen, 40 Sekunden Verdauung, Originalclips und Vorschauen, Pause/Spielgeschwindigkeit, Tod, Bungee, Boardwechsel und Speichern/Laden während Biss/Verdauung. Gewöhnlichen Chomper und vorhandene Explosionen gegenprüfen.

Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.30.dll` nach `Mods/FusionMod.dll` kopieren. Spielstände mit Cherry Chomper benötigen 0.1.31 oder eine kompatible neuere Version.

## Änderung 0.1.30: Cherry-nut, Solar Bomb und Cluster Mine

Alle drei Ergebnisse haben im vollständigen 4.0.5.2-Quellenarchiv jeweils genau ein Rezept. Beide Zutaten sind jeweils native, bereits verfügbare Pflanzen. Die Rezepte gelten in beiden Reihenfolgen für Karten und Handschuh:

```text
Cherry Bomb | Wall-nut | Cherry-nut
Cherry Bomb | Sunflower | Solar Bomb
Cherry Bomb | Potato Mine | Cluster Mine
```

Cherry-nut besitzt 4000 Lebenspunkte. Beim Tod löst sie eine Kirsch-Explosion mit 1800 Schaden aus. Freundliche Kirsch-Explosionen heilen überlebende Cherry-nuts im Wirkbereich um 200, höchstens bis 4000: normale Cherry Bomb, Solar Bomb, andere Cherry-nuts und die kleinen Explode-o-shooter-Explosionen sind angeschlossen. Gewöhnliche Kirschkugeln sind keine Explosionen. Freundliche Explosionen verursachen nativ bereits keinen Pflanzenschaden; es gibt keine allgemeine Immunität gegen Zombieangriffe oder Jack-in-the-Box. Die Originalkörper wechseln bei den nativen Schadensschwellen zwischen unbeschädigt und zwei Rissstufen. Heilung stellt die passende Darstellung wieder her.

Solar Bomb verwendet den gespeicherten nativen Cherry-Bomb-Zünder von einer Sekunde. Die Explosion verursacht 1800 Schaden und erzeugt für jeden getroffenen Zombie eine echte Sonne mit 25 Sonnenwert, auch wenn der Zombie überlebt. Schilde zählen nicht als zusätzliche Zombies. Zerstörung vor Zünderablauf löst dieselbe Explosion aus. Die Mod zählt gültige Schadensziele selbst: Der native Helfer `KillAllZombiesInRadius` liefert getötete Zombies und wäre für diese Sonnenregel falsch.

Cluster Mine ist eine essbare, niedrige Pflanze. Die tatsächliche Quellklasse `CherryMine` erbt von `Plant`, nicht von `PotatoMine`; ihr Controller hat nur `idle`. Daher löst sie nicht wie eine normale scharfe Potato Mine durch Berührung aus und besitzt keinen eigenen Ausgrabungszyklus. Beim Tod verursacht sie 1800 Minenschaden in ihrer Reihe und verteilt gewöhnliche Potato Mines auf die zulässigen Felder im 3×3-Bereich einschließlich des freigewordenen Mittelfelds. Jede erzeugte Mine erhält fünf Sekunden gespeicherten Ausgrabungs-Countdown; anschließend läuft ihre native Aufsteh-Animation bis zur Scharfschaltung. Belegte Felder, ungültiges Terrain und Felder außerhalb des Boards werden übersprungen. Bestehende Pflanzen, Seerosen, Töpfe und Kürbisse werden nicht ersetzt. Erzeugte Minen verwenden die normale Replanted-Darstellung und lösen keine weiteren Fusionen aus.

Neue Kirsch-Explosionen verwenden einen Kreisradius von zwei Feldern (160 Board-Pixel) mit höchstens einer Nachbarreihe; die Minenexplosion einen Radius von einem Feld (80 Pixel) in derselben Reihe. Native Zielberechtigungen und Zombie-Trefferrechtecke bleiben maßgeblich. Explode-o-shooter behält seinen bestehenden Radius und Schaden. Die normale Cherry Bomb bleibt bei ihrem nativen Radius von 115 Pixeln; ihre neue Nussheilung verwendet denselben Radius. Schaden trifft Schild und Körper und unterdrückt zurückbleibende Körper. Die neuen Explosionen löschen keine Leitern. Todeseffekte laufen erst nach erfolgreichem nativen Tod. Zutatenverbrauch, Vorschauen, getragene Pflanzen und Board-Abbau lösen keine Explosionen oder Minenverteilung aus. Eine gewöhnliche Entfernung einer lebenden Pflanze auf dem Board, etwa mit der Schaufel, zählt dagegen als Tod; dies ist manuell mitzuprüfen.

### Originalassets und Speicherung

Die Pflanzenmodelle stammen vollständig aus dem lokalen `PlantsVsZombiesRH_Data/data.unity3d`:

- Cherry-nut: `CherryNutPrefab` 36039, Transform 114130, Script `globalgamemanagers.assets:445 CherryNut`, Controller 6592; Clips 4905 `idle` und 4906 `nothing`; 12 Knoten, 9 Sprites.
- Solar Bomb: `SunBombPrefab` 50022, Transform 116955, Script `globalgamemanagers.assets:3758 SunBomb`, Controller 6633; Clips 5008 `Bomb`, 5009 `idle`, 5010 `shake`; 11 Knoten, 10 Sprites. Der Bomb-Clip folgt dem nativen Zünder; die additive Shake-Ebene bleibt erhalten. Animationsevents erzeugen keine zweite Explosion.
- Cluster Mine: `CherryMinePrefab` 57083, Transform 103897, Script `globalgamemanagers.assets:3088 CherryMine`, Controller 6378; Clip 4356 `idle`; 29 Knoten, 29 Sprites einschließlich des animierten roten Blinklichts.

Originalpivots, Hierarchien, Sprite-Reihenfolge, Kurven und Schattenbezug verwenden den vorhandenen Importweg einschließlich Handschuh- und Rastervorschau. Alle Modelle und Sprites sind eingebettet. Explosionsblitz, Geräusch, Sonnen und erzeugte normale Minen verwenden Replanted-Assets; Original-Partikel und Original-Audio werden nicht importiert. `nothing` ist der Original-Leerzustand für Angriffe; der Port verwendet den Original-Leerlauf und die nativen Schadensanzeigen, ohne ein zusätzliches Quellen-Angriffssystem einzuführen.

Persistierte Kennungen in `mImitaterType`: `Wallnut + Cherrybomb`, `Cherrybomb + Sunflower`, `Potatomine + Cherrybomb`. Diese gültigen Enum-Paare sind zentral reserviert und dürfen nicht wiederverwendet werden. Native Felder speichern Identität, Gesundheit, Zünder und Minen-Countdowns. Darstellung wird nach Laden neu aufgebaut. Es entsteht kein zusätzlicher flüchtiger Spielzustand dieser drei Pflanzen; Spielstände mit diesen Varianten benötigen mindestens diese Mod-Version. Bestehende Speichergrenzen fliegender Mod-Projektile bleiben bestehen.

Quellenrezepte mit Registrierungsadressen, chinesischer Original-Almanach, Methodenbelege, Prefab-/Animator-Daten, Asset-Prüfsummen und Offlinevorschauen liegen unter `references/cherry-nut/`, `references/solar-bomb/` und `references/cluster-mine/`. Gemeinsame aktuelle Plant-/PotatoMine-Lebenszyklen sind unter `references/cluster-mine-lifecycle/` archiviert. Diese Beschreibung übersetzt die maßgeblichen Almanachmerkmale. Export und Vorschau unterstützen die gleichnamigen `--plant`-Werte. Das vollständige Quellenarchiv bleibt unverändert. Aktuell: 32 aktive Rezepte; 636 offene Rezepte für 530 Ergebnisse.

### Manuelle Benutzerprüfung und Rückbau

Release-Build: 0 Warnungen, 0 Fehler. Alle 51 neuen eingebetteten Assetdateien stimmen mit den Exportdateien überein. Versionsattribute und Binärgleichheit der Installation sind statisch geprüft; Nachweis: `references/release-0.1.30.json`. Diese Prüfungen sind keine Spieltests. Das Spiel wurde nicht gestartet; Spielstände und Profile wurden nicht geändert. Der Benutzer prüft:

- Alle drei Rezepte in beiden Kartenreihenfolgen und beiden Handschuhrichtungen innerhalb des nativen Cherry-Bomb-Zündfensters; Kosten, Cooldown, Abbruch, ungültige Ziele und sichtbare Hervorhebung.
- Cherry-nut: 4000 Lebenspunkte, beide Rissstufen, Tod/Schaufel/Zerquetschen, 1800 Explosionsschaden, 200 Heilung durch jede angeschlossene Kirsch-Explosion, Heilungsgrenze und Rückkehr aus Rissstufen. Kein Heilungseffekt durch gewöhnliche Kirschkugeln.
- Solar Bomb: ein Zünder, eine Explosion, 25 Sonne pro getroffenem Zombie einschließlich Überlebender, keine zusätzlichen Sonnen für Schilde, normale Sonnensammlung und Sun Collector, Explosion bei vorzeitiger Zerstörung.
- Cluster Mine: keine selbstständige Scharfschaltung, Explosion beim Tod, freie 3×3-Felder einschließlich Mitte, Randfelder, belegte Felder, Wasser/Dach, erhaltene Unterlagen/Kürbisse, fünf Sekunden Countdown mit anschließender nativer Aufsteh-Animation.
- Originalgrafik, Blinklicht, Zündanimation, Größe/Bodenlage, beide Vorschauen, Pause/Spielgeschwindigkeit, Tragen, Bungee, Tod, Boardwechsel und Speichern/Laden. Normale Wall-nut, Cherry Bomb und Potato Mine sowie bestehende Fusionen gegenprüfen.

Installation: `Mods/FusionMod.dll`, Version 0.1.30. Rückbau bei geschlossenem Spiel: `backup/FusionMod-0.1.29.dll` zurückkopieren. Spielstände mit den neuen Varianten erst wieder mit 0.1.30 oder einer kompatiblen neueren Version laden. LawnGlove bleibt unverändert.

## Liste

`fusionen.txt` enthält **288 offene Rezepte** mit **230 Ergebnis-Pflanzen** im Format:

```text
Pflanze1 | Pflanze2 | Fusionname
```

Alphabetisch nach Pflanze1, danach Pflanze2 und Fusionname sortiert, ohne Beachtung der Groß-/Kleinschreibung. Namen entsprechen der englischen Übersetzung. Bereits fusionierte Pflanzen sind als Zutaten enthalten; jede Zeile beschreibt einen unmittelbaren Fusionsschritt.

Die Reihenfolge der Zutaten in der Portierungsliste entspricht den ursprünglichen Quellregistrierungen. In dieser Mod gelten implementierte Rezepte grundsätzlich in beiden Zutatenreihenfolgen. Gegenstände wie Bucket oder Football Helmet belegen ebenfalls die zweite Spalte.

Enthalten sind normale Rezepte, Reise-/Spezialrezepte sowie Gegenstandsfusionen. Sieben Rezepte behalten eine vorhandene Ergebnis-Pflanze bei, darunter die sechs registrierten Kombinationen von Basispflanze und zugehörigem Upgrade. Diese stehen für vollständige Kompatibilität ebenfalls in der Liste. Freischaltungen, Modusbedingungen und zusätzliche Voraussetzungen werden durch das gewünschte Dreispaltenschema nicht abgebildet. Die Liste beschreibt registrierte Rezepte, keine Garantie, dass jede Kombination in jedem Modus verfügbar ist. Andere Transformationen außerhalb der Rezeptregistrierung sind nicht als Zweipflanzenfusionen ergänzt.

## Herkunft

Die Zutaten und Ergebnisse wurden **statisch aus der lokalen `GameAssembly.dll`** unter `pvzfusion/english-4.0.5.2/Game Files/` ausgelesen. Grundlage sind die Aufrufe von `MixData.AddOrderedRecipe` (RVA `0x477d20`), `MixData.AddRecipe` (RVA `0x477f30`) und die 27 Einträge von `Bucket.Recipes` (Initialisierung ab RVA `0x9d5630`). Die sieben datenabhängigen Registrierungsstellen in `FirstMix` übernehmen diese Gegenstandsrezepte. Umgekehrte Registrierungen gleicher Rezepte wurden zusammengefasst.

Die vorhandenen generierten C#-Assemblies enthalten veraltete Methodenadressen. Deshalb wurden die tatsächlichen Aufrufstellen und Funktionsgrenzen der lokalen DLL ausgewertet. Der lokale Almanach-Dump enthält 720 Einträge; die DLL enthält zusätzlich ein Rezept mit Ergebnis-ID 1488. Auch dieses Rezept ist erfasst.

Die englischen Namen lädt der mitgelieferte Übersetzungsmod aus dem Projekt `Teyliu/PVZF-Translation`. Für diese Liste ist die [Namensquelle auf Commit 43982fb929e7](https://raw.githubusercontent.com/Teyliu/PVZF-Translation/43982fb929e71ce51911015fb7b01831c939694e/PvZ_Fusion_Translator/Localization/English/Almanac/LawnStringsTranslate.json) festgehalten. Die Onlinequelle dient ausschließlich den Namen; die Rezepte stammen aus der lokalen DLL.

`fusionen-quellen.json` archiviert den ursprünglichen Gesamtbestand mit 668 Rezepten und dokumentiert die Pflanzen-IDs, erlaubte Reihenfolgen und Registrierungsadressen sowie SHA-256-Prüfsummen der Quellen. Bei Gegenstandsrezepten verweist die Adresse auf den entsprechenden Eintrag in `Bucket.Recipes`.

## Stand

Das Grundgerüst ist implementiert. Aktive Rezepte:

```text
Blover | Sunflower | Solar Blover
Flower Pot | Sunflower | Solar Pot
Jalapeno | Sunflower | Solar Flare
Sea-shroom | Sunflower | Sunset-shroom
Pea-shroom | Bucket | Buck-shroom
Puff-shroom | Buckshooter | Buck-shroom
Puff-shroom | Cabbage-pult | Sprout-pult
Puff-shroom | Kernel-pult | Baby Corn
Puff-shroom | Melon-pult | Slice-pult
Puff-shroom | Sunflower | Sun-shroom
Puff-shroom | Hypno-shroom | Gnome-shroom
Puff-shroom | Umbrella Leaf | Umbrella Cap
Hail-shroom | Peashooter | Icicle-shroom
Pea-shroom | Ice-shroom | Icicle-shroom
Puff-shroom | Ice-shroom | Hail-shroom
Puff-shroom | Potato Mine | Nugget-shroom
Puff-shroom | Snow Pea | Icicle-shroom
Puff-shroom | Starfruit | Twinkle-shroom
Puff-shroom | Doom-shroom | Boom-shroom
Puff-shroom | Jalapeno | Bell Pepper
Puff-shroom | Marigold | Penny-shroom
Puff-shroom | Wall-nut | Nut-shroom
Pea-shroom | Peashooter | Repeat-shroom
Pea-shroom | Repeater | Split Pea-shroom
Pea-shroom | Split Pea | Gatling Pea-shroom
Puff-shroom | Gatling Pea | Gatling Pea-shroom
Puff-shroom | Peashooter | Pea-shroom
Puff-shroom | Repeater | Repeat-shroom
Puff-shroom | Split Pea | Split Pea-shroom
Repeat-shroom | Peashooter | Split Pea-shroom
Repeat-shroom | Repeater | Gatling Pea-shroom
Split Pea-shroom | Peashooter | Gatling Pea-shroom
Tall-nut | Football Helmet | Football-nut
Gatling Pea | Football Helmet | Football Gatling
Blover | Chomper | Flytrapper
Blover | Doom-shroom | Fallout Blover
Garlic | Hypno-shroom | Hypno Garlic
Starfruit | Sunflower | Solarfruit
Gatling Pea | Hypno-shroom | Gatling Hypno
Hypno Pea | Peashooter | Hypno Repeater
Hypno Pea | Repeater | Split Hypno
Hypno Pea | Split Pea | Gatling Hypno
Hypno Repeater | Peashooter | Split Hypno
Hypno Repeater | Repeater | Gatling Hypno
Hypno-shroom | Peashooter | Hypno Pea
Hypno-shroom | Repeater | Hypno Repeater
Hypno-shroom | Split Pea | Split Hypno
Peashooter | Split Hypno | Gatling Hypno
Blover | Scaredy-shroom | Camo-shroom
Scaredy-shroom | Starfruit | Starry-shroom
Scaredy-shroom | Sunflower | Sunny-shroom
Scaredy-shroom | Wall-nut | Sturdy-shroom
Cherry Bomb | Scaredy-shroom | Blasty-shroom
Ice-shroom | Scaredy-shroom | Shivery-shroom
Peashooter | Scaredy-shroom | Speedy-shroom
Potato Mine | Scaredy-shroom | Spuddy-shroom
Cherry Bomb | Doom-shroom | Black Cherry
Cherry Bomb | Ice-shroom | Frozen Cherry
Doom-shroom | Ice-shroom | Demise-shroom
Ice-shroom | Sunflower | Sundae-shroom
Flame-shroom | Gloom-shroom | Flame Gloom-shroom
Frost-shroom | Gloom-shroom | Frost Gloom-shroom
Fume-shroom | Gloom-shroom | Gloom-shroom
Fume-shroom | Scaredy-shroom | Gutsy-shroom
Gloom-shroom | Ice-shroom | Frost Gloom-shroom
Gloom-shroom | Jalapeno | Flame Gloom-shroom
Garlic | Threepeater | Stench-spreader
Ice-shroom | Threepeater | Snow Threepeater
Jalapeno | Scorched Threepeater | Charred Threepeater
Jalapeno | Threepeater | Scorched Threepeater
Fume-shroom | Garlic | Foul-shroom
Fume-shroom | Potato Mine | Starch-shroom
Iceweed | Spikerock | Icerock
Scorchweed | Spikerock | Hearthrock
Spikerock | Ice-shroom | Icerock
Spikerock | Jalapeno | Hearthrock
Chomper | Doom-shroom | Doom Chomper
Chomper | Ice-shroom | Frostbiter
Chomper | Squash | Jawbreaker
Doom-shroom | Squash | Doom Squash
Fume-shroom | Ice-shroom | Frost-shroom
Fume-shroom | Jalapeno | Flame-shroom
Doom-shroom | Fume-shroom | Soot-shroom
Fume-shroom | Threepeater | Fume-spreader
Squash | Starfruit | Starry Squash
Squash | Sunflower | Solar Squash
Tangle Kelp | Doom-shroom | Doom Kelp
Tangle Kelp | Jalapeno | Spicy Kelp
Kernel-pult | Umbrella Leaf | Umbrella Husk
Kernel-pult | Wall-nut | Butter-nut
Potato Mine | Sea-shroom | Land-mine
Sunflower | Torchwood | Torchflower
Garlic | Wall-nut | Foul-nut
Peashooter | Squash | Pea Squash
Squash | Bucket | Iron Squash
Starfruit | Wall-nut | Star-nut
Garlic | Gatling Pea | Gatling Poison
Garlic | Peashooter | Poison Pea
Garlic | Repeater | Poison Repeater
Garlic | Split Pea | Split Poison
Peashooter | Poison Pea | Poison Repeater
Peashooter | Poison Repeater | Split Poison
Peashooter | Split Poison | Gatling Poison
Poison Pea | Repeater | Split Poison
Poison Pea | Split Pea | Gatling Poison
Poison Repeater | Repeater | Gatling Poison
Cactus | Wall-nut | Thorn-nut
Hypno-shroom | Potato Mine | Hypno Mine
Ice-shroom | Potato Mine | Ice Mine
Jalapeno | Potato Mine | Chip Mine
Ice-shroom | Spikeweed | Iceweed
Jalapeno | Spikeweed | Scorchweed
Spikeweed | Sunflower | Solarweed
Spikeweed | Wall-nut | Spike-nut
Flame Wall-nut | Tall-nut | Flame Tall-nut
Frost Wall-nut | Tall-nut | Frost Tall-nut
Ice-shroom | Wall-nut | Frost Wall-nut
Jalapeno | Wall-nut | Flame Wall-nut
Tall-nut | Ice-shroom | Frost Tall-nut
Tall-nut | Jalapeno | Flame Tall-nut
Cherry Bomb | Squash | Cherry Squash
Hypno-shroom | Squash | Hypno Squash
Ice-shroom | Squash | Icy Squash
Jalapeno | Squash | Spicy Squash
Marigold | Marigold | Twin Marigold
Umbrella Leaf | Wall-nut | Umbrella Shell
Chomper | Potato Mine | Chomp-mine
Chomper | Sunflower | Solar Chomper
Sunflower | Wall-nut | Solar-nut
Solar-nut | Wall-nut | Twin Solar-nut
Peashooter | Potato Mine | Peatato Mine
Potato Mine | Sunflower | Solar Mine
Potato Mine | Wall-nut | Mine-nut
Peashooter | Bucket | Buckshooter
Wall-nut | Bucket | Buck-nut
Peashooter | Wall-nut | Pea-nut
Chomper | Peashooter | Chomp-shooter
Chomper | Wall-nut | Chomp-nut
Chomp-nut | Peashooter | Chompzilla
Chomp-shooter | Wall-nut | Chompzilla
Chomper | Pea-nut | Chompzilla
Cherry Bomb | Chomper | Cherry Chomper
Cherry Bomb | Wall-nut | Cherry-nut
Cherry Bomb | Sunflower | Solar Bomb
Cherry Bomb | Potato Mine | Cluster Mine
Cherry Bomb | Peashooter | Cherryshooter
Cherry Bomb | Cherryshooter | Explode-o-shooter
Cherry Bomb | Split Pea | Split Cherry
Cherrypeater | Peashooter | Split Cherry
Cherryshooter | Repeater | Split Cherry
Cherry Bomb | Gatling Pea | Gatling Cherry
Cherrypeater | Repeater | Gatling Cherry
Cherryshooter | Split Pea | Gatling Cherry
Peashooter | Split Cherry | Gatling Cherry
Cherry Bomb | Repeater | Cherrypeater
Cherryshooter | Peashooter | Cherrypeater
Peashooter | Sunflower | Solar-shooter
Sunflower | Sunflower | Twin Sunflower
Peashooter | Peashooter | Repeater
Peashooter | Repeater | Split Pea
Peashooter | Split Pea | Gatling Pea
Repeater | Repeater | Gatling Pea
Wall-nut | Wall-nut | Giant Wall-nut
Wall-nut | Tall-nut | Tall-nut
Ice-shroom | Peashooter | Snow Pea
Ice-shroom | Repeater | Snow Repeater
Peashooter | Snow Pea | Snow Repeater
Ice-shroom | Split Pea | Split Snow
Peashooter | Snow Repeater | Split Snow
Repeater | Snow Pea | Split Snow
Gatling Pea | Ice-shroom | Gatling Snow
Peashooter | Split Snow | Gatling Snow
Repeater | Snow Repeater | Gatling Snow
Snow Pea | Split Pea | Gatling Snow
Blaze Pea | Peashooter | Blaze Repeater
Blaze Pea | Repeater | Split Blaze
Blaze Pea | Split Pea | Gatling Blaze
Blaze Repeater | Peashooter | Split Blaze
Blaze Repeater | Repeater | Gatling Blaze
Gatling Pea | Jalapeno | Gatling Blaze
Jalapeno | Peashooter | Blaze Pea
Jalapeno | Repeater | Blaze Repeater
Jalapeno | Split Pea | Split Blaze
Peashooter | Split Blaze | Gatling Blaze
Cherry Bomb | Threepeater | Cherry-spreader
Ice-shroom | Melon-pult | Winter Melon
Lily Pad | Cattail | Cattail
Magnet-shroom | Marigold | Gold Magnet
Spikeweed | Spikerock | Spikerock
Garlic | Marigold | Silver Garlic
Ice-shroom | Marigold | Silver Ice-shroom
Marigold | Sunflower | Silver Sunflower
Marigold | Wall-nut | Silver-nut
Cabbage-pult | Marigold | Silver Cabbage
Doom-shroom | Marigold | Silver Doom-shroom
Hypno-shroom | Marigold | Silver Hypno-shroom
Marigold | Melon-pult | Silver Melon
Flower Pot | Marigold | Silver Pot
Flower Pot | Plantern | Lumos Pot
Kernel-pult | Marigold | Silver Kernel
Marigold | Umbrella Leaf | Silver Umbrella
Flower Pot | Cabbage-pult | Cabbage Pot
Flower Pot | Kernel-pult | Butter Pot
Flower Pot | Umbrella Leaf | Umbrella Pot
Kernel-pult | Cob Cannon | Cob Cannon
Flower Pot | Garlic | Garlic Pot
Flower Pot | Melon-pult | Melon Pot
Flower Pot | Spikeweed | Spike Pot
Flower Pot | Wall-nut | Nut Pot
Flower Pot | Cherry Bomb | Cherry Pot
Flower Pot | Doom-shroom | Doom Pot
Flower Pot | Hypno-shroom | Hypno Pot
Flower Pot | Jalapeno | Hot Pot
Flower Pot | Peashooter | Pea Pot
Flower Pot | Threepeater | Tri-pot
Kernel-pult | Threepeater | Kernel Tri-pult
Melon-pult | Threepeater | Melon Tri-pult
Cabbage-pult | Kernel-pult | Taco-pult
Cabbage-pult | Melon-pult | Overripe Melon
Cabbage-pult | Threepeater | Cabbage Tri-pult
Kernel-pult | Melon-pult | Cob-pult
Cabbage-pult | Cob-pult | Salad-pult
Cabbage-pult | Ice-shroom | Coldslaw
Cabbage-pult | Jalapeno | Kimchi-pult
Ice-shroom | Kernel-pult | Cobsicle
Kernel-pult | Overripe Melon | Salad-pult
Melon-pult | Taco-pult | Salad-pult
Cabbage-pult | Wall-nut | Cabbage-nut
Melon-pult | Wall-nut | Melon-nut
Squash | Wall-nut | Squash-nut
Threepeater | Wall-nut | Three-nuts
Tangle Kelp | Puff-shroom | Tingle Kelp
Tangle Kelp | Fume-shroom | Grapple Kelp
Tangle Kelp | Wall-nut | Tangle-nut
Tangle Kelp | Spikeweed | Spike-kelp
Eyes Zombie Head | Sunflower | Blood Sunflower
Blover | Blover | Cyclover
Blover | Garlic | Disguster
Blover | Hypno-shroom | Mind-blover
Blover | Ice-shroom | Breeze Blover
Fume-shroom | Peashooter | Boost-shroom
Fume-shroom | Spikeweed | Caltrop-shroom
Fume-shroom | Torchwood | Blowtorch-shroom
Fume-shroom | Wall-nut | Wallop-shroom
Cabbage-pult | Garlic | Garbage-pult
Garlic | Kernel-pult | Clove-pult
Garlic | Melon-pult | Garlic-pult
Jalapeno | Melon-pult | Summer Melon
Garlic | Starfruit | Starlic
Ice-shroom | Starfruit | Winter Starfruit
Jalapeno | Starfruit | Spicy Starfruit
Peashooter | Starfruit | Starshooter
Cattail | Ice-shroom | Frost Cattail
Cattail | Jalapeno | Flame Cattail
Hypno-shroom | Jalapeno | Hypno Pepper
Hypno-shroom | Melon-pult | Hypno Melon
Sea-shroom | Cactus | Urchin-shroom
Sea-shroom | Fume-shroom | Shark-shroom
Sea-shroom | Puff-shroom | Minnow-shroom
Sea-shroom | Starfruit | Sea Star
Sea-shroom | Hypno-shroom | Jelly-shroom
Sea-shroom | Ice-shroom | Swell-shroom
Sea-shroom | Plantern | Beacon-shroom
Sea-shroom | Scaredy-shroom | Hermit-shroom
Pumpkin | Cactus | Thorny Pumpkin
Pumpkin | Fume-shroom | Fuming Pumpkin
Pumpkin | Puff-shroom | Puff-kin
Pumpkin | Starfruit | Starry Pumpkin
Pumpkin | Cherry Bomb | Cherry Pumpkin
Pumpkin | Garlic | Stinky Pumpkin
Pumpkin | Scaredy-shroom | Scareless Pumpkin
Pumpkin | Wall-nut | Nut Shell
Pumpkin | Blover | Windy Pumpkin
Pumpkin | Plantern | Jack O' Plantern
Pumpkin | Sea-shroom | Sea-shell
Pumpkin | Spikeweed | Spiky Pumpkin
Cabbage-pult | Squash | Lettuce-pult
Fume-shroom | Melon-pult | Spring Melon
Kernel-pult | Squash | Butternut-pult
Melon-pult | Squash | Gourd-pult
Doom-shroom | Magnet-shroom | Dormant-shroom
Fume-shroom | Magnet-shroom | Morph-shroom
Magnet-shroom | Melon-pult | Melon Morph
Magnet-shroom | Starfruit | Starmorph
Cactus | Fume-shroom | Saguaro-shroom
Cactus | Ice-shroom | Ice Cactus
Cactus | Magnet-shroom | Magnet Cactus
Plantern | Wall-nut | Illumi-nut
Doom-shroom | Garlic | Doom Garlic
Doom-shroom | Potato Mine | Crater-tot
Doom-shroom | Sunflower | Doomflower
Doom-shroom | Wall-nut | Doom-nut
Pumpkin | Ice-shroom | Cryo Pumpkin
Pumpkin | Jalapeno | Pyro Pumpkin
Pumpkin | Melon-pult | Melon Shell
Pumpkin | Peashooter | Pumpkin Pod
Kernel-pult | Magnet-shroom | Kernel Morph
Melon Morph | Bucket | Metal-pult
Morph-shroom | Bucket | Rust-shroom
Starmorph | Bucket | Iron Star
Doom Pea | Split Pea | Gatling Doom
Doom-shroom | Gatling Pea | Gatling Doom
Doom-shroom | Peashooter | Doom Pea
Doom-shroom | Starfruit | Death Star
Doom-shroom | Threepeater | Doom-spreader
Cabbage-pult | Doom-shroom | Damage-pult
Cactus | Umbrella Leaf | Umbrella Thorn
Fume-shroom | Starfruit | Starburst-shroom
Garlic | Umbrella Leaf | Umbrella Clove
Fume-shroom | Kernel-pult | Blubber-shroom
Fume-shroom | Plantern | Ray-shroom
Fume-shroom | Sunflower | Sunrise-shroom
Fume-shroom | Umbrella Leaf | Bush-shroom
Beamshooter | Peashooter | Rebeamer
Beamshooter | Repeater | Split Beam
Beamshooter | Split Pea | Gatling Beam
Gatling Pea | Plantern | Gatling Beam
Peashooter | Plantern | Beamshooter
Peashooter | Rebeamer | Split Beam
Peashooter | Split Beam | Gatling Beam
Plantern | Repeater | Rebeamer
Plantern | Split Pea | Split Beam
Rebeamer | Repeater | Gatling Beam
Cactus | Doom-shroom | Doom Cactus
Cactus | Plantern | Lumos Cactus
Cactus | Spikeweed | Thornweed
Cactus | Starfruit | Starthorn
Kernel-pult | Spikeweed | Butterweed
Melon-pult | Spikeweed | Springweed
Sea-shroom | Squash | Splash
Sea-shroom | Wall-nut | Sea-nut
Spikeweed | Squash | Squashweed
Squash | Threepeater | Squash-spreader
Tangle Kelp | Squash | Strangle Kelp
Tangle Kelp | Threepeater | Kelp-spreader
Potato Mine | Squash | Potato Masher
Potato Mine | Tangle Kelp | Tangle Mine
Sea-shroom | Potato Mine | Sea-mine
Tangle Kelp | Potato Mine | Kelp Mine
Fume-shroom | Hypno-shroom | Perfume-shroom
Spikeweed | Tangle Kelp | Tangle Weed
Blover | Plantern | Glowver
Doom-shroom | Scaredy-shroom | Frenzy-shroom
Cabbage-pult | Sunflower | Solar-pult
Charred Threepeater | Jalapeno | Phoenix Threepeater
Ice-shroom | Plantern | Aurora Plantern
Plantern | Threepeater | Triplantern
Flower Pot | Blover | Blover Pot
Flower Pot | Scaredy-shroom | Scaredy Pot
Flower Pot | Sea-shroom | Sea Pot
Peashooter | Threepeater | Multipeater
Blover | Cabbage-pult | Cabbage Dropper
Blover | Kernel-pult | Butter Fly
Puff-shroom | Fume-shroom | Puff-shroom Spreading: Fume-shroom
Puff-shroom | Scaredy-shroom | Puff-shroom Spreading: Scaredy-shroom
Puff-shroom | Cactus | Ball Cactus
Puff-shroom | Cherry Bomb | Cherry Pop
Cherry Bomb | Fume-shroom | Trigger-shroom
Chomper | Fume-shroom | Belch-shroom
Chomper | Scaredy-shroom | Hungry-shroom
Ice-shroom | Jalapeno | Spicicle
Demise-shroom | Fume-shroom | Glacier-shroom
Doom-shroom | Frost-shroom | Glacier-shroom
Ice-shroom | Soot-shroom | Glacier-shroom
Pumpkin | Chomper | Chompkin
Pumpkin | Potato Mine | Pumpkin Mine
Pumpkin | Sunflower | Sunny Pumpkin
Marigold | Potato Mine | Marigold Mine
Potato Mine | Threepeater | Threetato Mine
Cabbage-pult | Umbrella Leaf | Umbrella Kale
Flower Pot | Ice-shroom | Ice Pot
Melon-pult | Umbrella Leaf | Umbrella Rind
Plantern | Umbrella Leaf | Lumos Umbrella
Doom-shroom | Plantern | Blacklight Plantern
Garlic | Torchwood | Stenchwood
Jalapeno | Torchwood | Scorchwood
Sea-shroom | Blover | Tide-shroom
Hypno-shroom | Torchwood | Hypnowood
Starfruit | Torchwood | Sparkwood
Threepeater | Torchwood | Torchthree
Torchwood | Wall-nut | Nutwood
```

Eine Sunflower aus der Samenkarte auf eine vorhandene Sunflower pflanzen. Das Spiel berechnet weiterhin die Kosten der eingehenden Sunflower (normalerweise 50 Sonne) und deren Karten-Cooldown. Die Fusion verwendet die vorhandene native Twin Sunflower mit deren normalem Verhalten; keine zusätzliche Twin-Sunflower-Karte oder Shop-Freischaltung ist nötig. Eine Sunflower auf einem freien Feld wird unverändert normal gepflanzt. Die ursprüngliche Twin-Sunflower-Upgrade-Karte bleibt nutzbar.

Alternativ eine Sunflower mit dem Handschuh aufnehmen und auf eine andere Sunflower absetzen. Beide Zutaten werden durch eine native Twin Sunflower am Zielfeld ersetzt; das Quellfeld wird frei. Der Handschuh verbraucht dabei keine zusätzliche Sonne und erhält den bisherigen 10-Sekunden-Cooldown. Ungültige Ziele und Abbruch behalten das bisherige Handschuhverhalten. Die Rastervorschau zeigt die getragene Ausgangspflanze.

Native Platzierungsregeln der Ergebnis-Pflanze gelten, einschließlich Seerose/Topf, Kratern, Spezialfeldern und Modusbeschränkungen. Die unter einer Zutat stehende Seerose bzw. der Topf sowie vorhandene Kürbisse werden nicht verbraucht. Beschädigte Zutaten ergeben eine frisch initialisierte Twin Sunflower; Lebenspunkte und Produktionszähler werden wie bei einer neuen nativen Pflanze initialisiert. Eine dritte Sunflower auf einer Twin Sunflower löst keine weitere Fusion aus. Zen-Garten und nicht registrierte Kombinationen bleiben unverändert. Imitater-Samenkarten sind in diesem ersten Rezept nicht als zusätzlicher Pflanzweg implementiert; eine bereits verwandelte Sunflower kann mit dem Handschuh verwendet werden.

## Aufbau und weitere Rezepte

- `src/FusionPlant.cs`: Rezeptidentität aus nativem Pflanzentyp und optionaler Variantenkennung; `From(plant)` unterscheidet vorhandene Fusionen von ihren nativen Grundtypen.
- `src/FusionRecipes.cs`: Zentrale Zuordnung `(Zutat A, Zutat B) -> Ergebnis`. Weitere Fusionen zu vorhandenen nativen Pflanzen, etwa Repeater + Repeater -> Gatlingpea, benötigen hier einen weiteren Eintrag. Jedes Rezept einmal eintragen; die Abfrage akzeptiert automatisch beide Zutatenreihenfolgen. Replanted-`SeedType` verwenden, nicht die Fusion-IDs aus `fusionen-quellen.json`.
- `src/FusionService.cs`: Gemeinsame Zielprüfung und Handschuh-Fusion. Ergebnis wird vor dem Entfernen der Zutaten erzeugt. Die Platzierung wird über die native Prüfung des Ergebnisses validiert; eine Rekursionssperre verhindert dabei erneute Rezeptauflösung.
- `src/PlantingPatches.cs`: Erweitert die native Zielprüfung und ändert ausschließlich innerhalb eines Spieler-Pflanzvorgangs den erzeugten Pflanzentyp. Kosten, Karten-Cooldown, Pflanzeffekte und Cursorverwaltung verbleiben beim Spiel. Skriptgesteuerte `AddPlant`-Aufrufe und Spielstandwiederherstellung lösen keine Fusion aus.
- `glove-mod/src/OccupiedTilePlacement.cs`: Öffentliche Ereignisse `CanPlace` und `TryPlace` für belegte Handschuhziele. Die Fusionsmod meldet ihre gemeinsame Logik dort an. Die Glove-Mod kennt keine Rezepte und benötigt die Fusionsmod nicht. Ohne registrierte Erweiterung bleiben belegte Ziele gesperrt.

Der aktuelle Adapter unterstützt Zutaten und Ergebnisse auf der normalen Pflanzenebene. Native Upgrades werden bei passender nativer Grundpflanze mit dieser geprüft. Andernfalls wird die zu verbrauchende Zutat für die synchrone native Terrainprüfung vorübergehend ausgeblendet und im finally-Block wiederhergestellt; für Upgrades gilt dann der gemeinsame Terrain-Pflanzentyp aus der Glove-Mod. Neue Pflanzentypen, andere Ebenen oder besondere Mehrfeldregeln benötigen zusätzlich eigene Platzierungs-/Erzeugungslogik im Service; dafür allein reicht ein Rezeptlisteneintrag nicht aus. Die Portierungsliste aktiviert keine weiteren Rezepte automatisch.

## Bauen und Installation

```bash
../.tools/dotnet/dotnet build FusionMod.csproj -c Release
```

Die Fusionsmod baut die Glove-Mod als Projektabhängigkeit mit. Ziel: .NET 6, vorhandene Replanted-/MelonLoader-Interop-Assemblies. Anderen Spielpfad gegebenenfalls mit `-p:GameDir=...` angeben.

Installierte Dateien:

- `Mods/FusionMod.dll` (0.1.29)
- `Mods/LawnGlove.dll` (1.2.2, für die neue Schnittstelle erforderlich)

Die vorherige installierte Glove-DLL ist unter `backup/LawnGlove-before-fusion.dll` gesichert. `SHA256SUMS` dokumentiert die gebauten DLLs. Zum Rückbau `Mods/FusionMod.dll` entfernen und bei Bedarf die gesicherte Glove-DLL als `Mods/LawnGlove.dll` zurückkopieren. Bereits erzeugte Twin Sunflowers sind native Pflanzen und erfordern für ihre Speicherung keine eigene Mod-Datenstruktur.

## Prüfung und offene Spieltests

Release-Build beider Projekte: 0 Warnungen, 0 Fehler. Die nativen Abläufe wurden statisch nachvollzogen: `Board.CanPlantAt` (RVA `0x6565b0`), `Board.MouseDownWithPlant` (`0x665440`), `Board.AddPlant` (`0x6553e0`), `Board.NewPlant` (`0x668e90`) und `Plant.IsUpgradableTo` (`0x6b2450`). Der native Sonnenabzug und die Kartenbenutzung erfolgen weiterhin für die eingehende Sunflower, bevor bzw. nach dem Erzeugen der Ergebnis-Pflanze.

Das Spiel wurde nicht gestartet, keine automatischen Tests ausgeführt und keine Spielstand-/Profildateien verändert. Laufzeitverhalten und Darstellung sind noch nicht bestätigt. Die Spieltests übernimmt der Benutzer: Kartenfusion mit/ohne genug Sonne, Handschuhfusion und Cooldown, Abbruch/ungültige Ziele, Platzierung auf Seerose/Topf bzw. unter Kürbis sowie Speichern/Laden der Twin Sunflower.


## Änderung 0.1.1: Fusionsziele leuchten

Während eine Sunflower aus dem Roster oder am Handschuh gehalten wird, leuchten alle gültigen Sunflower-Fusionsziele mit derselben nativen Farbe wie beim Schaufel-/Handschuh-Hover. Die getragene Pflanze selbst wird nicht markiert. Andere Pflanzen, ungültige Zielfelder und nicht implementierte Rezepte bekommen kein zusätzliches Leuchten. Beim Absetzen, Abbruch oder Werkzeugwechsel berechnet der nächste native Farbupdate wieder die normale Darstellung. Während Pause wird kein Fusionsleuchten angefordert.

`src/FusionHighlightPatch.cs` erweitert `Plant.UpdateReanimColor`: Das native `mHighlighted`-Flag wird ausschließlich während des Farbupdates für ein gültiges Fusionsziel gesetzt und danach auf seinen ursprünglichen Wert zurückgestellt. Vorhandene Hover-Markierungen bleiben dadurch erhalten; es werden keine dauerhaften Flags im Spielstand gespeichert. Der native Animationsupdate ruft den Farbupdate regelmäßig auf (statisch bestätigt, Aufrufstelle RVA `0x6bb922`). Die Fusionsprüfung verwendet dieselben Rezepte und Platzierungsregeln wie der Commit. Die Glove-Mod 1.2.1 stellt mit `LawnGloveMod.GetCarriedPlant(Board)` eine schreibgeschützte Abfrage der gerade getragenen Pflanze bereit.

Beide Release-Builds: 0 Warnungen, 0 Fehler. Kein Spielstart und keine Spieltests ausgeführt. Bitte insbesondere mehrere mögliche Ziele, unveränderten normalen Schaufel-/Handschuh-Hover, Aufnahme/Abbruch, Werkzeugwechsel und erfolgreiche Fusion prüfen. Vorherige installierte DLLs: `backup/FusionMod-0.1.0.dll` und `backup/LawnGlove-1.2.0.dll`.


## Änderung 0.1.2: Peashooter + Peashooter -> Repeater

Zwei Peashooter fusionieren zu einer frisch initialisierten nativen Repeater. Das funktioniert über die Peashooter-Samenkarte (normale Peashooter-Kosten, üblicherweise 100 Sonne, und Karten-Cooldown) sowie mit dem Handschuh (keine zusätzlichen Sonnenkosten, normaler Handschuh-Cooldown). Passende Peashooter-Ziele leuchten beim Halten eines Peashooters. Die Sunflower-Fusion bleibt aktiv.

Repeater ist kein natives Upgrade. Deshalb verwendet die gemeinsame `CheckFusionPlacement`-Prüfung bei normalen Ergebnissen dieselben vorübergehenden Ausschlussflags wie die Glove-Mod: Nur die zu verbrauchende Zielpflanze wird während der synchronen nativen Terrainprüfung mit `mSquished=true` und `mIsOnBoard=false` ausgeschlossen. Beide Werte werden auch bei Fehlern wiederhergestellt. Unterstützende Pflanzen und Kürbisse bleiben sichtbar für die native Prüfung und werden nicht verbraucht. Bei nativen Upgrade-Ergebnissen bleibt die Grundpflanze für deren Upgrade-Prüfung vorhanden. Alle drei Wege (Roster, Glove, Leuchten) verwenden diese gemeinsame Prüfung.

Release-Build: 0 Warnungen, 0 Fehler. Kein Spielstart und keine Spieltests ausgeführt. Noch durch den Benutzer zu prüfen: Roster-/Glove-Fusion, Ziel-Leuchten, Sonnenkosten/Karten-Cooldown, ungültige Ziele, Seerose/Topf/Kürbis sowie die bestehende Sunflower-Fusion. Vorversion: `backup/FusionMod-0.1.1.dll`.


## Änderung 0.1.3: Split Pea und vertauschbare Zutaten

Peashooter + Repeater und Repeater + Peashooter ergeben eine native Split Pea. Alle Rezeptabfragen akzeptieren grundsätzlich beide Reihenfolgen, auch für künftige Rezepte: Ein einziger Eintrag in `FusionRecipes.cs` genügt. Roster, Glove und Ziel-Leuchten verwenden diese Abfrage gemeinsam. Beim Roster wird die tatsächlich hinzugefügte Karte bezahlt und nachgeladen (Peashooter normalerweise 100, Repeater normalerweise 200 Sonne); der Handschuh verbraucht wie bisher keine zusätzliche Sonne.

`SplitPeaProjectiles.cs` erfasst ausschließlich Projektile, die während `Plant.Fire` aus der Sekundärwaffe einer Split Pea erzeugt werden. Deren linke Schüsse werden am linken Bildschirmrand in Board-Koordinaten (`-board.mX`) gespiegelt und auf `ProjectileMotion.Straight` umgeschaltet. Dies erhält die native Geschwindigkeit von 3,33 Einheiten pro Tick; `mVelX` allein würde die feste native Rückwärtsbewegung nicht ändern. Die Prüfung läuft nach der Bewegung, vor der nativen Kollisions-/Offscreen-Prüfung. Kollisionsschaden und Torchwood-Konvertierung bleiben beim Spiel. Rechte Split-Pea-Schüsse und fremde Rückwärtsprojektile werden nicht markiert. Das gilt für alle nativen Split Peas, auch bereits vorhandene Pflanzen.

Die Zuordnung wird bei Projektiltod, Pool-Reset und Board-Entsorgung entfernt, damit wiederverwendete Projektile keine alte Markierung erhalten. Einschränkung: Die native Projektilserialisierung speichert keinen Schützen. Nach dem Laden eines Spielstands haben bereits gespeicherte, noch rückwärts fliegende Projektile daher keine Rückprallmarkierung; neu abgefeuerte Split-Pea-Projektile erhalten sie regulär. Bereits umgekehrte Projektile bleiben nativ vorwärts fliegend gespeichert.

Der Eintrag `Peashooter | Repeater | Split Pea` wurde aus `fusionen.txt` entfernt. Andere offene Rezepte, die Split Pea als Zutat verwenden, bleiben erhalten. Das Quellenarchiv bleibt vollständig.

Release-Build: 0 Warnungen, 0 Fehler. Kein Spielstart und keine Spieltests ausgeführt. Offene Benutzerprüfung: beide Reihenfolgen per Roster und Glove, passende Zielmarkierungen, linker Schuss am Rand (auch nach Torchwood), unveränderte rechte Schüsse und andere Pflanzen. Vorversion: `backup/FusionMod-0.1.2.dll`.


## Änderung 0.1.4: Alle Gatling-Pea-Rezepte

Die beiden im Quellenarchiv registrierten Rezepte mit Ergebnis Gatling Pea sind implementiert:

```text
Peashooter | Split Pea | Gatling Pea
Repeater | Repeater | Gatling Pea
```

Beide gelten unabhängig von der Zutatenreihenfolge, über Roster und Glove, mit derselben Zielmarkierung wie alle anderen Rezepte. Ergebnis ist eine frisch initialisierte native Gatling Pea. Beim Roster werden Kosten und Karten-Cooldown der tatsächlich hinzugefügten Zutat verwendet. Der Handschuh verwendet seinen bisherigen Cooldown ohne zusätzliche Sonnenkosten.

Die native Gatling-Pea-Upgrade-Regel akzeptiert nur Repeater als Grundpflanze. `CheckFusionPlacement` verwendet diese Regel, wenn die vorhandene Zutat nativ zum Ergebnis aufrüstbar ist. Für die anderen Fusionszutaten wird das Terrain bei vorübergehend ausgeschlossener Zielpflanze anhand des normalen Grundtyps der Ergebnis-Pflanze geprüft. Die Zuordnung stammt aus der bereits vorhandenen Glove-Logik, jetzt gemeinsam unter `glove-mod/src/PlantPlacementRules.cs`; sie wurde nicht dupliziert. Unterstützende Pflanzen und Kürbisse bleiben bei Prüfung und Fusion erhalten. Roster, Glove und Leuchten verwenden dieselbe Prüfung.

Die beiden erledigten Einträge wurden aus `fusionen.txt` entfernt. Andere Gatling-Varianten und Rezepte, die Gatling Pea als Zutat benötigen, bleiben offen. Das Quellenarchiv bleibt vollständig.

Release-Build beider DLLs: 0 Warnungen, 0 Fehler. Kein Spielstart und keine Spieltests ausgeführt. Benutzerprüfung: beide Rezepte, beide Zutatenreihenfolgen beim Peashooter-/Split-Pea-Rezept, Roster/Glove und Ziel-Leuchten, Terrain/Unterpflanzen sowie die bisherigen Fusionen. Vorversionen: `backup/FusionMod-0.1.3.dll` und `backup/LawnGlove-1.2.1.dll`.


## Änderung 0.1.5: Giant Wall-nut und gekoppelte Split-Pea-Köpfe

`Wall-nut + Wall-nut -> Giant Wall-nut` ist über Roster und Glove implementiert. Beide Zutaten müssen in der optisch unbeschädigten Stufe sein. Die Prüfung entspricht exakt der nativen `AnimateNuts`-Grenze: `health >= floor(2 * maxHealth / 3)`. Bei den normalen 4000 Lebenspunkten ist das ab 2666 Lebenspunkten erlaubt. Kleinere Bissschäden ohne sichtbare Risse sind zulässig. Eine rissige Ziel-Wall-nut und eine rissige Glove-Zutat verhindern Fusion und Fusionsleuchten. Die normale native Reparatur einer beschädigten Wall-nut per Samenkarte bleibt möglich und erzeugt keine Giant Wall-nut.

`FusionRecipes.AcceptsIngredient` besitzt die zustandsabhängige Rezeptbedingung; `FusionService` wendet sie für Zielpflanzen und getragene Zutaten an. Damit prüfen Platzierung, Commit und Leuchten dieselbe Schadensgrenze.

`GiantWallNut.cs` verwendet den vorhandenen nativen Pflanzentyp `GiantWallnut`, dessen Controller und die native Bowling-Ground-Animation `_ground`. Die Größe wird absolut auf das Doppelte gesetzt, einschließlich 160 × 160 logischer Größe. Die Bewegung verwendet die native Ground-Geschwindigkeit bei normaler Bowling-Referenzrate 12 fps, rechnet den visuellen Größenfaktor heraus und halbiert den normalen ganzzahligen Bewegungsschritt. Dadurch verdoppelt die größere Darstellung nicht die Geschwindigkeit. Die Giant Wall-nut rollt nach rechts, verändert weder Reihe noch vertikale Position, aktualisiert ihre aktuelle Spalte und verschwindet hinter dem rechten Board-Rand.

Ein überstrichenes Kollisionsrechteck prüft alle Zombies derselben Lane, statt nur ein Ziel pro Tick zu beschädigen. Jeder getroffene lebende Zombie stirbt über die native `DieWithLoot`-Funktion, unabhängig von Lebenspunkten, Helm oder Schild. Der native Bowling-Trefferton wird bei Treffern gespielt. Während Pause oder Tragen mit dem Glove bewegt sie sich nicht. Die Implementierung ersetzt nur den Fähigkeitsupdate des Giant-Wall-nut-Typs außerhalb des Zen-Gartens; andere Bowling-Nüsse behalten ihr bisheriges Verhalten. Identität, Position und Controller verwenden die native Speicherung, ohne zusätzliche Mod-Spielstanddatei.

Der Giant-Wall-nut-Eintrag wurde aus der offenen Portierungsliste entfernt; das Quellenarchiv bleibt erhalten.

`SplitPeaLinkedFire.cs` koppelt beide Split-Pea-Waffen: Bei jedem nativen Schuss wird der entsprechende Gegenschuss direkt ausgelöst, auch ohne eigenes Ziel auf dieser Seite. Die nativen Mündungspositionen und Projektiltypen bleiben erhalten. Eine Rekursionssperre verhindert eine endlose Schusskette. Eine Maske pro `UpdateShooting` verhindert doppelte Gegenschüsse, wenn das Spiel beide Waffen im selben Update auslösen würde. Mehrere Spielupdates pro gerendertem Frame werden getrennt behandelt. Die Rückprallmarkierung der linken Split-Pea-Schüsse funktioniert auch für die zusätzlich ausgelösten Schüsse.

Release-Build: 0 Warnungen, 0 Fehler. Keine Spielstarts oder Spieltests durch den Agenten. Benutzerprüfung: Giant-Fusion aus unbeschädigten bzw. leicht angebissenen Nüssen; Ausschluss beider rissigen Glove-/Zielzutaten; normales Reparieren; Leuchten, Größe, Geschwindigkeit, feste Lane, Treffer gegen mehrere gepanzerte/starke Zombies, Pause und Speichern/Laden. Split Pea: nur links/ nur rechts/ beidseitig vorhandene Ziele sowie Rückprall der neu gekoppelten linken Schüsse. Vorversion: `backup/FusionMod-0.1.4.dll`.


## Korrektur 0.1.6: Native Split-Pea-Salve und Schussanimationen

Die direkte Schusskopplung aus 0.1.5 wurde ersetzt. Die Split Pea verwendet wieder vollständig ihren nativen Schussplan: **ein Schuss nach vorn, zwei zeitversetzte Schüsse nach hinten**, mit beiden nativen Kopfanimationen. Das gilt auch dann, wenn nur links oder nur rechts ein Ziel steht. Ohne Ziel auf beiden Seiten beginnt keine neue Salve.

`SplitPeaLinkedFire.cs` erweitert jetzt ausschließlich die Zielsuche innerhalb von `Plant.FindTargetAndFire`. Findet ein Split-Pea-Kopf dort kein Ziel, darf ein natives Ziel auf der anderen Seite seinen regulären Schussablauf aktivieren. Die zusätzliche Richtungsabfrage besitzt eine Rekursionssperre und bleibt auf die konkrete Pflanze und diesen Methodenaufruf beschränkt. Andere Zielabfragen behalten ihre native Bedeutung. Die bisherigen Fire-Hooks für zusätzliche Gegenschüsse und die UpdateShooting-Maske wurden entfernt; zusätzliche Projektile oder eigene Salven-Timer werden nicht mehr erzeugt.

Statisch nachvollzogen: Native `UpdateShooter` startet den hinteren Kopf am Anfang des Zyklus und startet später vorderen und hinteren Kopf zusammen (LaunchCounter 25). `FindTargetAndFire` startet die Animationen `shooting` (Head) und `splitpea_shooting` (Head2). `UpdateShooting` erzeugt die jeweiligen Projektile zum nativen Animationszeitpunkt. Die bestehende Markierung und Richtungsumkehr der linken Projektile bleibt aktiv.

Release-Build: 0 Warnungen, 0 Fehler. Kein Spielstart oder Spieltest ausgeführt. Benutzerprüfung: nur Ziel rechts, nur Ziel links, Ziele auf beiden Seiten; je Zyklus 1 Projektil vorn und 2 hinten, sichtbare Kopfanimationen und linker Rückprall. Vorversion: `backup/FusionMod-0.1.5.dll`.


## Korrektur 0.1.7: Wall-nut-Reparatur verbrauchte die Fusionszutat vorzeitig

Ursache des fehlenden Giant-Wall-nut-Spawns: Nach erfolgreicher Platzierungsprüfung ruft der native `MouseDownWithPlant`-Ablauf bei Wall-nut auf Wall-nut die Reparaturlogik auf. Diese entfernt die vorhandene Nuss über `Plant.Die` (RVA `0x665e92`), bevor `Board.AddPlant` erreicht wird. Die bisherige Fusionssuche fand dadurch keine lebende Ausgangspflanze und ließ wieder eine normale Wall-nut erzeugen.

`PlayerPlantingScope` merkt sich jetzt für den konkreten Pflanzklick die bereits validierte Fusionszutat. Der neue `DeferFusionIngredientDeathPatch` verschiebt ausschließlich deren Entfernung innerhalb dieses synchronen Vorgangs. Der vorhandene AddPlant-Hook findet dadurch die Zutat, erzeugt das Ergebnis und entfernt erst danach die Ausgangspflanze über einen explizit freigegebenen Consume-Aufruf. Der Vorgangskontext wird durch einen Harmony-Finalizer auch bei Fehlern wiederhergestellt. Unbeteiligte Pflanzen, normale Reparaturen beschädigter Nüsse und der Glove-Ablauf behalten ihre bisherige Todes-/Verbrauchslogik. Kartenkosten und Karten-Cooldown bleiben nativ.

Die Bedingung ohne sichtbaren Schaden gilt weiterhin. Rissige Wall-nuts werden nicht als Fusionszutaten vorgemerkt und können weiterhin normal repariert werden.

Release-Build: 0 Warnungen, 0 Fehler. Reparaturpfad statisch anhand der lokalen GameAssembly nachvollzogen; kein Spielstart oder Spieltest ausgeführt. Benutzerprüfung nach vollständigem Neustart: Wall-nut-Samenkarte auf eine intakte Wall-nut, leicht angebissene aber rissfreie Nuss, normale Reparatur einer rissigen Nuss sowie Glove-Fusion. Vorversion: `backup/FusionMod-0.1.6.dll`.


## Korrektur 0.1.8: Giant Wall-nut bleibt auf dem Feld und verursacht Kontaktschaden

Die bisherige Abbruchprüfung verwendete `board.mWidth`, die Widget-Breite. Stattdessen gilt jetzt dieselbe logische rechte Grenze wie beim nativen `Plant.UpdateBowling`: X = 800 (Vergleich bei RVA `0x6b7df5`). Dadurch wird die rollende Nuss nicht mehr allein wegen einer unpassenden Widget-Breite vorzeitig entfernt. Doppelte Größe, halbe normale Bowling-Geschwindigkeit und feste Lane bleiben erhalten.

Die Soforttötung aus 0.1.5 wurde durch **1800 Schaden pro direkt berührtem Zombie und Nuss** ersetzt. `Zombie.TakeDamage(1800, HitsShieldAndBody | DoesntLeaveBody)` verwendet den nativen Explosionsschadenswert und dessen Schadensflags (0x12), ohne eine Explosion, einen Verbrennungseffekt oder eine Umkreissuche auszulösen. Ein Gargantuar mit vollen Lebenspunkten überlebt den Treffer. Eine Menge getroffener Zombie-DataIDs pro Nuss verhindert weitere Treffer gegen denselben überlebenden Zombie während des Weiterrollens. Pflanzen-/Board-Identität und Pool-IDs grenzen diese Zuordnung ab; Tod, Reset und Board-Entsorgung räumen sie auf.

Einschränkung beim Speichern/Laden: Die Trefferhistorie lebt nur im Arbeitsspeicher. Wird eine rollende Nuss mit einem noch überlappenden überlebenden Zombie gespeichert und später geladen, kann dieser erneut getroffen werden. Position und Pflanzentyp werden weiterhin nativ gespeichert.

Release-Build: 0 Warnungen, 0 Fehler. Native Randprüfung und Schadensaufruf statisch nachvollzogen; kein Spielstart oder Spieltest ausgeführt. Benutzerprüfung nach vollständigem Neustart: sichtbares Weiterrollen bis zum rechten Rand, feste Lane, Größe/Geschwindigkeit, mehrere Kontakte und genau ein 1800er-Treffer pro Nuss gegen einen voll geheilten Gargantuar. Vorversion: `backup/FusionMod-0.1.7.dll`.


## Änderung 0.1.9: Wall-nut + Tall-nut → Tall-nut

Genau ein weiteres Rezept aus der offenen Liste ist implementiert:

```text
Wall-nut | Tall-nut | Tall-nut
```

Beide Zutaten sind native, bereits benutzbare Replanted-Pflanzen (`SeedType.Wallnut = 3`, `SeedType.Tallnut = 23`). Die Fusion erzeugt eine frische native Tall-nut mit 8000 Lebenspunkten und ihrer nativen Sperre gegen normale Stabhochsprung-, Pogo- und Delfinsprünge. Beschädigte Zutaten dürfen verwendet werden; die Sonderbedingung ohne sichtbare Risse bleibt auf Giant Wall-nut beschränkt. Ergebnis, Controller, Animationen, Schadensstufen und Speicherung sind bereits nativ vorhanden; ein zusätzliches Verhaltenspatch ist für dieses Rezept nicht erforderlich.

Wie alle Mod-Rezepte gilt es in beiden Reihenfolgen: Tall-nut-Karte auf Wall-nut oder Wall-nut-Karte auf Tall-nut, ebenso beide Richtungen mit dem Handschuh. Passende Ziele verwenden das bestehende Fusionsleuchten. Kartenkosten und Karten-Cooldown gehören zur eingehenden Karte (normalerweise Tall-nut 125 bzw. Wall-nut 50 Sonne). Der Handschuh verbraucht keine zusätzliche Sonne und behält seinen Cooldown. Beide Zutaten werden verbraucht, Unterstützer/Kürbisse erhalten. Normales Pflanzen auf freien Feldern bleibt möglich. Die bestehende synchrone Terrainprüfung und Erzeugung vor Zutatenverbrauch werden unverändert wiederverwendet.

### Quelle und Assets

Maßgeblich ist der lokale Build `../pvzfusion/english-4.0.5.2/Game Files/`. Das unveränderte Quellenarchiv belegt die Registrierung bei RVAs `0x47ec9d` und `0x480fd5`: Wall-nut (Fusion-ID 3) + Tall-nut (Fusion-ID 1027) → Tall-nut (1027). Diese IDs nicht mit den Replanted-Typen verwechseln. Der lokale `Mods/PvZ_Fusion_Translator/Dumps/LawnStrings.json`-Eintrag 1027 beschreibt 8000 Lebenspunkte, die hohe Sprungsperre und Wall-nut als Basis. Der relevante Originaleintrag samt deutscher Funktionsbeschreibung, Quellenprüfsummen und Rezeptnachweis liegt unter `references/tall-nut/`.

Die Originaltexturen `Tallnut_body`, `Tallnut_cracked1` und `Tallnut_cracked2` werden aus `PlantsVsZombiesRH_Data/data.unity3d` als Referenzassets dort abgelegt. Für das Spiel verwendet dieses native Ergebnis den bereits passenden Replanted-Controller und seine Grafiken; die Fusion-Unity-Texturen ersetzen keine Replanted-Animationen. Die Darstellung ist daher die native Replanted-Tall-nut, keine pixelgenaue Übernahme des Fusion-Artworks. Es gibt keine zusätzlich zu installierenden Assetdateien.

Bewusste bestehende Mod-Regeln gegenüber der Quelle: Beide Zutatenreihenfolgen sind erlaubt; Tall-nut bleibt auch auf freien Feldern nativ pflanzbar. Die Fusion-Quelle beschreibt die Wall-nut als Upgrade-Basis. Ihre einseitige Registrierung wird im Quellenarchiv unverändert bewahrt.

### Prüfung und manueller Spieltest

Release-Build erfolgreich: 0 Warnungen, 0 Fehler. Die installierte FusionMod-DLL stimmt per SHA-256 mit dem Build überein; Vorversion 0.1.8 ist gesichert. Keine automatischen Tests, kein Spielstart und keine Spielstand-/Profiländerungen; Laufzeit und Darstellung prüft der Benutzer. Bitte nach vollständigem Neustart beide Kartenreihenfolgen und beide Handschuhrichtungen prüfen, dazu Ziel-Leuchten, Sonnenkosten/Cooldowns, beschädigte Zutaten (frisches Ergebnis), gültige Seerosen-/Topffelder mit Kürbis, ungültige Ziele und Abbruch, Speichern/Laden sowie die bisherigen Wall-nut-/Giant-Wall-nut-Rezepte. Für die Funktion insbesondere Lebenspunkte und die Sprungsperre prüfen.

Der erledigte Eintrag wurde aus `fusionen.txt` entfernt; das Quellenarchiv bleibt vollständig. Der neue verbindliche Workflow steht in [vorgehen-neue-fusionen.md](vorgehen-neue-fusionen.md), einschließlich Quellversions-, Asset-, ID- und Reparaturfallstricken. Vorversion/Rückbau: `backup/FusionMod-0.1.8.dll` zurück nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt bei 1.2.2.


## Änderung 0.1.10: Snow Pea und Gefrierpunkte

`Ice-shroom | Peashooter | Snow Pea` ist implementiert. Beide Zutaten sind bereits native Replanted-Pflanzen (`Iceshroom = 14`, `Peashooter = 0`); Ergebnis ist `Snowpea = 5`. Nicht mit den Fusion-Quell-IDs 10, 0 und 1034 verwechseln. Beide Kartenreihenfolgen und beide Handschuhrichtungen verwenden die vorhandene zentrale Rezeptauflösung, Terrainprüfung und Zielmarkierung. Das Ergebnis entsteht vor dem Verbrauch der Zutaten. Seerosen, Töpfe und Kürbisse bleiben erhalten. Kosten und Cooldown gehören zur eingehenden Karte; der Handschuh behält seine bisherigen Regeln. Eine schlafende Ice-shroom kann als vorhandene Zutat dienen; tagsüber aktiviert sie ihren Effekt vor der Fusion nicht. Nachts muss eine vorhandene Ice-shroom vor ihrem nativen Auslösen fusioniert werden.

Snow Pea verwendet den nativen Controller, seine Schussanimationen und Eisprojektile (20 Basisschaden, ungefähr 1,5 Sekunden pro Schuss). `src/SnowPea.cs` ergänzt den im lokalen 4.0.5.2-Almanach beschriebenen Gefriermechanismus: Jeder Eis-Erbsentreffer gibt einem einfrierbaren Zombie 10 Gefrierpunkte. Bei zunächst 100 Punkten friert er vier Sekunden ein. Die Schwelle steigt nach jeder durch Punkte ausgelösten Einfrierung um 100 bis maximal 1000. Treffer während einer Einfrierung verlängern diese nicht und übertragen keine Punkte in den nächsten Zyklus. Bei Auftauen oder Entfernen der Kälte werden Restpunkte verworfen; die erhöhte Schwelle bleibt für diesen Zombie erhalten. Bereits gefrorene Zombies erhalten vierfachen Eis-Erbsenschaden, auch beim Treffer, der die Gefrierschwelle erreicht (normalerweise 80 statt 20). Andere Geschosse erhalten keinen Schadensbonus. Die nativen Rüstungs-, Schild- und Immunitätsregeln bleiben zuständig.

Der native `Zombie.HitIceTrap` erzeugt Einfrierung, Eiscontroller, Geräusche und Animationsstillstand. Sein eigener zusätzlicher 20er-Ice-shroom-Schaden wird nur beim Aufruf durch Gefrierpunkte unterdrückt; die Erbse verursacht weiterhin genau ihren eigenen Treffer. Die Gefrierdauer dieses gemeinsamen Wegs beträgt nun vier Sekunden plus zehn Sekunden anschließende Kälte, auch für die normale Ice-shroom. Längere schon bestehende Effekte werden nicht verkürzt. Normale Eis-Erbsentreffer verwenden weiterhin die native zehnsekündige Verlangsamung. Die Darstellung bleibt die native blaue Kältefärbung mit Eisfessel; eine pixelgenaue Übernahme des Fusion-Eises ist damit nicht verbunden.

Diese Anpassung gilt für alle nativen Snow Peas, einschließlich direkt gepflanzter, schon vorhandener und geladener Pflanzen. Entscheidend ist der Projektiltyp beim Aufprall. Ein durch Torchwood bereits in eine gewöhnliche Erbse verwandeltes Geschoss gibt weder Gefrierpunkte noch den Bonus. Die Änderung erfordert keine zusätzliche Markierung der Pflanze oder gespeicherten Projektile.

### Quellen und Assets

`references/snow-pea/` enthält die lokalen Almanacheinträge 1034 und 10, das unveränderte Quellenrezept (Registrierung RVA `0x47cece`), Originaltexturen für Snow Pea, Eis-Erbse und Eisfessel sowie Herkunft und Prüfsummen. Die statische Auswertung der tatsächlichen lokalen DLL belegt `Bullet_snowPea.HitZombie` bei `0x826ce0`, Gefrierpunkte bei `0x64dd50` und die Anfangsschwelle bei `0x65fbf8`. Die aktuelle Quelle erhöht die Schwelle bis **1000**; das ältere inoffizielle Unity-Projekt nennt 400 und wurde hierfür nicht übernommen. Tatsächliche Methodentokens wurden aus der lokalen Metadatendatei aufgelöst; generierte Fusion-Interop-Tokens sind teilweise verschoben.

Wie bei anderen nativen Ergebnissen werden die bereits passenden Replanted-Assets zur Laufzeit verwendet. Die extrahierten Fusion-Texturen sind Referenzassets und keine austauschbaren Replanted-Animationen. Zusätzliche Assetdateien müssen nicht ins Spiel installiert werden.

### Speichergrenzen und manuelle Prüfung

Gefrierpunkte und erhöhte Schwellen liegen ausschließlich im Arbeitsspeicher. Nach Speichern/Laden beginnen sie erneut bei 0/100. Laufende native Kälte-/Gefrierzähler und native Snow-Pea-Pflanzen werden vom Spiel gespeichert. Zombie-Tod, Pool-Reset und Board-Ende bereinigen den zusätzlichen Zustand; Board und DataID schützen gegen wiederverwendete Objekte. Es werden keine Spielstände oder Profile durch die Modinstallation geändert.

Release-Build erfolgreich: 0 Warnungen, 0 Fehler. Installierte DLL und Build sind per SHA-256 binärgleich; Assembly- und MelonInfo-Version wurden statisch als 0.1.10 geprüft. Vorversion 0.1.9 ist gesichert. Keine automatischen Tests und kein Spielstart; Laufzeitprüfung übernimmt der Benutzer. Bitte nach vollständigem Neustart prüfen:

- Beide Kartenreihenfolgen und Handschuhrichtungen, Ziel-Leuchten, Kosten/Cooldown, Abbruch und ungültige Ziele; Seerose/Topf/Kürbis.
- Tagsüber schlafende Ice-shroom als Zutat; nachts deren kurzer Zeitraum bis zum Auslösen.
- Gegen einen ausreichend widerstandsfähigen, einfrierbaren Zombie: erste Einfrierung nach zehn Eis-Erbsentreffern, vier Sekunden Stillstand, anschließend Verlangsamung; zweite Einfrierung nach zwanzig weiteren Treffern nach dem Auftauen.
- Vierfacher Schaden während Einfrierung und beim auslösenden Treffer; kein zusätzlicher 20er-Schaden durch die Eisfessel. Normale Peashooter bleiben bei normalem Schaden.
- Ice-shroom allein: vier Sekunden Einfrierung, zehn Sekunden anschließende Kälte und weiterhin ihr normaler Schaden. Frostimmune Zombies, Schilde und Helme, Feuer und Torchwood, Pause sowie Speichern/Laden.
- Bestehende Fusionen, insbesondere Split Pea und Giant Wall-nut.

Der erledigte Eintrag wurde aus `fusionen.txt` entfernt; 660 Rezepte mit 542 Ergebnissen bleiben offen. Das Quellenarchiv bleibt unverändert. Vorversion/Rückbau: `backup/FusionMod-0.1.9.dll` nach `Mods/FusionMod.dll` zurückkopieren. LawnGlove bleibt bei 1.2.2.


## Änderung 0.1.11: Snow Repeater mit beiden Rezepten

Die hier beschriebene native Ersatzdarstellung galt nur für 0.1.11. Ab 0.1.12 werden Originalgrafik und Originalanimationen eingebunden; siehe folgenden Änderungsabschnitt.

Alle zwei im lokalen Quellenarchiv registrierten Rezepte für dieses Ergebnis sind implementiert:

```text
Ice-shroom | Repeater | Snow Repeater
Peashooter | Snow Pea | Snow Repeater
```

Alle vier Zutaten sind bereits benutzbar. Beide Rezepte funktionieren in beiden Reihenfolgen über Karten und Handschuh; Ziel-Leuchten, Terrainprüfung, Seerosen/Töpfe/Kürbisse und Erzeugung vor Zutatenverbrauch verwenden weiterhin den gemeinsamen Fusionsweg. Karten bezahlen nur die eingehende Zutat und verwenden deren Cooldown. Der Handschuh behält seine bisherigen Kosten- und Cooldownregeln. Beschädigte Zutaten ergeben eine frisch initialisierte Pflanze mit 300 Lebenspunkten. Die Einschränkung bei einer nachts sofort auslösenden Ice-shroom bleibt bestehen; eine tagsüber schlafende Ice-shroom lässt sich normal als Zutat nutzen.

Snow Repeater verwendet die native Repeater-KI: zwei zeitversetzte Schüsse pro Salve, ungefähr alle 1,5 Sekunden. Nur seine Geschosse werden **vor** der nativen Projektilinitialisierung auf Eis-Erbsen umgestellt. Jede verursacht 20 Basisschaden und verwendet die vorhandene Snow-Pea-Logik: Kälte, 10 Gefrierpunkte, vier Sekunden Einfrieren bei erreichter Schwelle und vierfacher Schaden gegen gefrorene Zombies einschließlich des auslösenden Treffers. Beide Geschosse tragen getrennt Gefrierpunkte bei. Immunitäten, Rüstungen, Torchwood, Pause und Zielerfassung bleiben nativ. Normale Repeater, Snow Peas und Split Peas erhalten keine zusätzlichen Schüsse oder fremden Projektiltypen.

### Variantenkennung, Darstellung und Speicherung

Ein eigener Quelltyp 1105 wird nicht an native Replanted-APIs übergeben. `FusionPlant.SnowRepeater` bezeichnet stattdessen das Paar aus `mSeedType = Repeater` (7) und `mImitaterType = Snowpea` (5). Dieses Paar entsteht nicht durch native Imitater-Verwandlung, deren Kennung 48 ist. Die native Initialisierung setzt beide Felder vor der Controller-Erzeugung. Die native Serialisierung speichert beide, die Deserialisierung liest sie vor dem Wiederaufbau des Controllers und Pool-Reset setzt die Kennung auf None zurück. Es gibt keinen zusätzlichen Varianten-Speicher im Arbeitsspeicher und keine Änderung am binären Speicherformat. Mit aktiver Mod soll Snow Repeater daher auch nach Laden und Handschuhverschieben erhalten bleiben; der Spieltest steht aus.

Die zentrale Rezeptauflösung verwendet bei vorhandenen und getragenen Pflanzen immer `FusionPlant.From(plant)`. Snow Repeater wird dadurch nicht als normaler Repeater in dessen Rezepten verbraucht. Beispielsweise darf Peashooter auf Snow Repeater keine normale Split Pea ergeben; das noch offene Ergebnis Split Snow bleibt gesperrt. Native Upgrade-Karten behalten ihre eigenen Spielregeln.

`SnowRepeater.cs` wählt bei der Controller-Erzeugung, auch beim Laden, den vorhandenen Replanted-Snow-Pea-Controller. Er besitzt die passenden Schussanimationen für den nativen Erbsenschützen; die Repeater-KI liefert die Doppelsalve. Die Darstellung ist damit eine native blaue Snow Pea mit Doppelschuss, keine pixelgenaue Übernahme des Fusion-Artworks. Beim Tragen verwendet der bestehende Handschuh den nativen Grundtyp als Vorschau: grüner Repeater; nach dem Absetzen erscheint wieder der Eis-Controller. Es sind keine zusätzlichen Assetdateien im Spiel nötig.

Gefrierpunkte und erhöhte Zombie-Schwellen behalten die in 0.1.10 dokumentierte Speichergrenze: Sie beginnen nach Laden wieder bei 0/100. Die Variantenkennung selbst bleibt nativ gespeichert. Ohne aktive FusionMod verhält sich die gespeicherte Pflanze wie ein gewöhnlicher Repeater; eine Rückkehr zu einer älteren Modversion stellt ebenfalls keinen Snow-Repeater-Effekt bereit. Zum Rückbau die gesicherte DLL verwenden; Spielstände werden durch die Installation nicht angefasst.

### Quelle und Grenzen

Maßgeblich ist ausschließlich `../pvzfusion/english-4.0.5.2/Game Files/`. Das Quellenarchiv belegt Ice-shroom (10) + Repeater (1030) bei RVA `0x47cee6` und Peashooter (0) + Snow Pea (1034) bei `0x47ce47`, jeweils Ergebnis 1105. Dies sind Quellen-IDs, keine Replanted-SeedTypes.

`references/snow-repeater/` enthält den unveränderten lokalen Almanacheintrag 1105, das aus `data.unity3d` extrahierte Originalbild `DoubleSnow` (Texture2D 2190, resources.assets), `DoubleSnowPrefab` (GameObject 35603) samt Komponenten, Quellenprüfsummen und statische Verhaltens-/Speicherbelege. Die tatsächlichen DLL-Adressen wurden anhand der aktuellen Metadaten und CodeGenModule-Tabelle aufgelöst: `DoubleSnowPeaShooter.Awake` bei `0x4d3d40`, `GetBulletType` bei `0x54c2d0`. Letzteres wählt regulär den Eis-Erbsentyp 15; die Sonderverzweigung mit einer weiteren, hier nicht implementierten Quellpflanze wird nicht portiert. Die bestehende gemeinsame Gefrierlogik ist in `references/snow-pea/` belegt.

Der Almanach nennt zusätzlich ein manuelles Klickupgrade nach 15 Sekunden zu **Split Snow**. Dieses andere Ergebnis und seine Rezepte bleiben offen; der Klickupgrade ist in dieser Version nicht verfügbar. Der ausgewählte Fusionsschritt erzeugt ausschließlich Snow Repeater. Beide erledigten Snow-Repeater-Zeilen wurden aus `fusionen.txt` entfernt; 658 Rezepte mit 541 Ergebnissen bleiben offen. Das vollständige Quellenarchiv bleibt unverändert. Der Workflow verlangt nun ausdrücklich, alle zulässigen Rezepte eines ausgewählten Ergebnisses gemeinsam zu berücksichtigen, und dokumentiert Variantenkennung, Projektile, Speicherung und diese Portierungsgrenzen.

### Prüfung und manueller Spieltest

Release-Build erfolgreich: 0 Warnungen, 0 Fehler. Installierte FusionMod-DLL und Build sind per SHA-256 binärgleich; Assembly- und MelonInfo-Version 0.1.11 wurden statisch geprüft. Vorversion 0.1.10 ist gesichert. LawnGlove bleibt unverändert bei 1.2.2. Keine automatischen Tests, kein Spielstart und keine Spielstand-/Profiländerungen. Laufzeit und Darstellung prüft der Benutzer nach vollständigem Neustart:

- Beide Rezepte, jeweils beide Kartenreihenfolgen und beide Handschuhrichtungen; Ziel-Leuchten, Kosten/Cooldown, beschädigte Zutaten, Abbruch und ungültige Ziele.
- Gültige Seerosen-/Topffelder und Kürbisse; tagsüber schlafende Ice-shroom sowie deren kurzer Zeitraum bis zum Auslösen nachts.
- Blaue Darstellung, zwei zeitversetzte Eis-Erbsen pro Salve, normales Zielverhalten; normale Repeater und Snow Peas im Vergleich.
- Kälte und Gefrierpunkte jedes Geschosses, vierfacher Schaden beim Einfrieren, Immunitäten, Schilde, Torchwood und Pause.
- Snow Repeater verschieben sowie Speichern/Laden: Variantenkennung, Darstellung und Doppelschuss erhalten; während einer Salve gespeicherte native Schusszähler weiterverwenden.
- Snow Repeater darf über Karten, Handschuh oder Leuchten keine normalen Repeater-Fusionsrezepte anbieten. Bestehende Repeater-/Split-Pea-/Giant-Wall-nut-Fusionen weiter prüfen.

Rückbau: `backup/FusionMod-0.1.10.dll` nach `Mods/FusionMod.dll` zurückkopieren.


## Korrektur 0.1.12: Originalgrafik und Originalanimationen für Snow Repeater

Snow Repeater verwendet jetzt die Grafik **und beide Animationsclips aus dem lokalen PVZ Fusion 4.0.5.2**. Die native Ersatzdarstellung aus 0.1.11 ist entfernt. Das gilt für neu fusionierte und geladene Snow Repeater sowie für beide Handschuhvorschauen. Beide Rezepte, Kostenregeln, Speicherung der Variantenkennung, Doppelschuss und Gefrierlogik bleiben erhalten.

### Originaldaten und Einbindung

Aus `PlantsVsZombiesRH_Data/data.unity3d`, Datei `resources.assets`, werden das originale `DoubleSnowPrefab` (GameObject 35603), sein AnimatorController `DoubleSnow` (6442), seine **19 Hierarchieobjekte mit 17 Sprites** und die Clips **idle (4541, 2 Sekunden)** sowie **shoot (4542, ca. 1,333 Sekunden)** übernommen. Dazu gehören einzelne Kopf-, Mund-, Augen-, Augenbrauen-, Eiskristall-, Stiel-, Blatt- und Schattensprites. Das statische Almanachbild `DoubleSnow.png` wird nicht als animierte Pflanze ausgegeben.

`assets/snow-repeater/` enthält die ausgeschnittenen Original-Sprites und `model.json` mit ursprünglichen Pivotpunkten, Pixels-per-Unit, Transform-Hierarchie, Sichtbarkeit, Zeichenreihenfolge und vollständigen kubischen Animationskurven. Diese Daten werden als 18 Ressourcen in die FusionMod-DLL eingebettet; zusätzliche Dateien müssen nicht in den Spielordner kopiert werden. `references/snow-repeater/visual-assets.json` dokumentiert Originalobjekte und SHA-256-Prüfsummen. Die Extraktion stammt ausschließlich aus dem vorgeschriebenen lokalen Build, nicht aus dem inoffiziellen Unity-Projekt.

`SnowRepeaterVisuals.cs` baut aus diesen reinen Darstellungsdaten Unity-SpriteRenderer auf. Der native Repeater-Controller bleibt für KI, Schusszeitsteuerung und Speicherung aktiv, seine Grafik und sein Schatten werden ausgeblendet. Der Originalschatten ist Teil der importierten Darstellung. Die Originalkurven werden direkt ausgewertet, einschließlich der Sichtbarkeitskanäle für die Blinkteile. Originalproportionen bleiben erhalten; die native Darstellungsgrenze dient ausschließlich zur Umrechnung in Replanteds Größen und Bodenposition. Die ursprüngliche interne Sprite-Reihenfolge bleibt in einer SortingGroup erhalten, deren äußere Sortierung dem nativen Controller folgt.

Der Leerlaufclip folgt dem nativen Board-Simulationszähler mit einer stabilen kosmetischen DataID-Phasenverschiebung und der im Quellcontroller angegebenen Geschwindigkeit 1,3. Der kurze native Pingpong-Framezähler der Pflanze wird dafür nicht als Uhr verwendet. Der Schussclip folgt der Phase der nativen `shooting`-Animation und wird auf die bestehende native Doppelschussfolge zeitlich angepasst. Seine Bewegungen stammen aus den Originalkurven. Die beiden originalen `AnimShoot`-Ereignisse (ca. 0,533 und 1,067 Sekunden) sind als Quellenbeleg archiviert, werden aber nicht als zusätzliche Spielaufrufe ausgeführt: Der native Repeater bleibt allein für die zwei Geschosse verantwortlich. Projektilflug, Eis-Erbsen-/Treffereffekte und Gefrierdarstellung bleiben wie in 0.1.11 nativ; diese Korrektur ersetzt die Pflanzen- und Vorschaudarstellung.

Beide Handschuhvorschauen verwenden ebenfalls die zusammengesetzten Original-Sprites und den Original-Leerlauf. Die native Vorschau bleibt für Cursor-/Rasterposition, Transparenz und Sortierung zuständig. Pause hält die Animationsphase an. Die Pflanzendarstellung folgt weiterhin den nativen Farbwerten für Markierung und Schaden. Native Controllerdaten werden nicht um zusätzliche GameObjects erweitert; nach Laden entsteht die Darstellung erneut aus der gespeicherten Variantenkennung. Tod, Pool-Reset, Controller-Ende, Board-Ende und Mod-Ende bereinigen die Darstellung. Fehler beim Aufbau werden geloggt und stellen die native Darstellung wieder her, statt die Pflanze unsichtbar zu lassen.

### Reproduktion und Assetvorschau

`tools/export_snow_repeater.py` benötigt Python mit UnityPy 1.25.4 und Pillow. Es liest das lokale Originalbundle und schreibt die eingebetteten Daten sowie den Herkunftsnachweis. Die Originalclips enthalten in diesem Player-Build keine gewöhnlichen Float-/Positionskurven; sie wurden vollständig aus `m_StreamedClip` einschließlich der Konstantkanäle rekonstruiert. Unbekannte Bindungen führen zum Abbruch des Exports.

`tools/preview_snow_repeater.py` rendert die extrahierte Hierarchie und ihre Kurven für eine visuelle Offline-Prüfung. Ergebnisse: [Originalanimation als GIF](references/snow-repeater/original-animation-preview.gif) und [Einzelbildübersicht](references/snow-repeater/original-animation-preview.png). Beide zeigen die Quelldaten, keine Aufnahme aus Replanted und keinen bestätigten Spieltest.

### Prüfung und offener Spieltest

Release-Build erfolgreich: 0 Warnungen, 0 Fehler. Eingebettete Assetdateien und Ressourcen wurden statisch abgeglichen. Installierte DLL und Build sind per SHA-256 identisch; Assembly- und MelonInfo-Version sind 0.1.12. Die Vorversion liegt unter `backup/FusionMod-0.1.11.dll`. LawnGlove bleibt unverändert bei 1.2.2.

Das Spiel wurde nicht gestartet. Keine automatischen Spieltests und keine Spielstand-/Profiländerungen. Bitte nach vollständigem Neustart insbesondere prüfen:

- Beide Rezepte und beide Zutatenreihenfolgen über Karte und Handschuh; Originalgrafik mit Eiskristallen und Augenbrauen.
- Original-Leerlauf und Schussbewegung, genau zwei Eis-Erbsen pro Salve, Pause sowie bisherige Kälte-/Gefrierfunktion.
- Cursor- und Rastervorschau beim Tragen, versteckte/ungültige Rasterziele, Transparenz, Absetzen, Abbruch und erneutes Aufnehmen.
- Bodenausrichtung und Größenverhältnis, Seerose/Topf/Kürbis, Schatten, Ziel-Leuchten und Schadensblinken; keine zusätzlich sichtbare native Repeater-Grafik.
- Speichern/Laden im Leerlauf und während einer Salve, Schaufeln/Tod und neuer Board-Aufbau; keine zurückbleibenden Grafikteile oder doppelte Schatten.

Der Split-Snow-Klickupgrade bleibt wie in 0.1.11 offen. Die offene Rezeptliste bleibt bei 658 Rezepten mit 541 Ergebnissen. Rückbau: `backup/FusionMod-0.1.11.dll` nach `Mods/FusionMod.dll` kopieren; sämtliche Originalassets dieser Korrektur stecken in der neuen DLL.


## Korrektur 0.1.13: Render-Material der Originalgrafik

Nach dem Benutzer-Spieltest war Snow Repeater mit 0.1.12 unsichtbar. Im vorhandenen MelonLoader-Laufzeitlog steht kein Fehler beim Laden der Originalassets; das vorhandene Unity-Playerlog belegt jedoch die aktive Universal Render Pipeline (URP). Der Import erzeugte SpriteRenderer ohne explizite Materialzuweisung. Damit war die Anbindung an Replanteds Pipeline unvollständig; allein dekodierte Sprites und gültige Animationskurven garantieren keine sichtbare Darstellung.

Alle importierten Pflanzenteile und beide Handschuhvorschauen erhalten jetzt ein gemeinsames Material mit dem im installierten Shaderbundle belegten `Universal Render Pipeline/2D/Sprite-Unlit-Default`. Der Shader muss vorhanden und unterstützt sein, bevor die native Ersatzdarstellung ausgeblendet wird. Das Material hat einen weißen Grundfarbwert; Originaltexturen, Alphakanäle, Sprite-Farben und Animationen bleiben unverändert. Der Container übernimmt außerdem denselben Kameralayer wie der native Renderer. Das Material wird mit den importierten GPU-Assets beim Mod-Ende freigegeben. Shader-Herkunft und Passdaten sind unter `references/snow-repeater/replanted-sprite-shader.json` dokumentiert.

Pro aufgebauter Darstellung meldet `Snow Repeater original art ready` einmalig Shader, Spriteanzahl, Kameralayer, Maßstab, native Grenzen, Sichtbarkeits-/Aktivflags und den nativen Farbwert. Scheitert die Shader-/Assetinitialisierung, bleibt die native Darstellung erhalten und der Fehler erscheint im Log. Die Materialkorrektur adressiert den statisch festgestellten fehlenden Render-Vertrag; ihre Wirkung im Spiel ist noch nicht bestätigt.

Release-Build: 0 Warnungen, 0 Fehler. Version 0.1.13 und die Binärgleichheit von Build und installierter DLL wurden statisch geprüft. Die vorherige installierte DLL liegt unter `backup/FusionMod-0.1.12.dll`; LawnGlove bleibt unverändert. Keine Änderungen an Rezepten, Originalassetdateien, Spielständen oder Profilen. Das Spiel wurde nicht gestartet und es wurden keine automatischen Spieltests ausgeführt.

Bitte nach vollständigem Neustart die Sichtbarkeit einer neu fusionierten und einer geladenen Snow-Repeater-Pflanze prüfen, dazu Original-Leerlauf/Schussbewegung und beide Handschuhvorschauen. Falls die Pflanze weiterhin unsichtbar bleibt, enthält `MelonLoader/Latest.log` jetzt die genannten Darstellungswerte zur Eingrenzung. Rückbau: `backup/FusionMod-0.1.12.dll` nach `Mods/FusionMod.dll` kopieren.


## Korrektur 0.1.14: Originalgrafik nach Shader-Ladevorgang aufbauen

Der Benutzer-Spieltest zu 0.1.13 zeigte gewöhnliche Repeater. Das aktuelle Log belegt jetzt den Grund ausdrücklich: `Required Snow Repeater shader is unavailable: Universal Render Pipeline/2D/Sprite-Unlit-Default`. Beide geladenen Pflanzen scheiterten bereits in der Szene `Loading`. Die Wiederherstellung der nativen Darstellung griff korrekt; die Originaldarstellung wurde anschließend jedoch nicht mehr aufgebaut.

`Shader.Find` lädt keinen Shader aus einem noch nicht geladenen Addressables-Bundle. Die Mod ermittelt deshalb jetzt den tatsächlich registrierten Shader über die nativen ResourceLocators und lädt dessen IResourceLocation mit `Addressables.LoadAssetAsync<Shader>`. Es wird kein Bundle parallel zur Spielverwaltung geöffnet. Abgeglichen wird der interne Assetpfad `Packages/com.unity.render-pipelines.universal/Shaders/2D/Sprite-Unlit-Default.shader`; die Katalogschlüssel selbst dürfen GUIDs, Labels oder beliebige Adressen sein. Herkunft und Assetpfad sind im vorhandenen installierten Shaderbundle belegt.

Der Aufbau beginnt erst in `Playing`. Solange der asynchrone Ladevorgang läuft, bleibt die Darstellung registriert und die native Grafik als vorübergehender Platzhalter sichtbar. Nach erfolgreichem Laden und Prüfung von Shadername und GPU-Unterstützung werden die unveränderten Original-Sprites und -Animationen aufgebaut und die native Grafik ausgeblendet. Warten ist damit kein Fehlerpfad mehr. Der Lade-Handle bleibt so lange wie die gemeinsam genutzten Materialien erhalten und wird bei Mod-Ende über Addressables freigegeben. Ein Boardwechsel während des Ladens benötigt keine verwaisten Callbacks; neu registrierte Pflanzen können den laufenden oder bereits abgeschlossenen Ladevorgang weiterverwenden.

Im Log steht zunächst `Loading Snow Repeater shader through Addressables`, nach abgeschlossenem Grafikaufbau `Snow Repeater original art ready`. Tatsächliche Ladefehler werden weiterhin ausdrücklich gemeldet und lassen die native Darstellung bestehen. Der Spieltest steht weiterhin aus.

Release-Build: 0 Warnungen, 0 Fehler. Version 0.1.14, Binärgleichheit von Build und installierter DLL sowie alle 18 unveränderten Originalasset-Ressourcen wurden statisch geprüft. Vorversion: `backup/FusionMod-0.1.13.dll`. LawnGlove bleibt unverändert; keine zusätzlichen DLLs oder Assetdateien müssen installiert werden. Das Spiel wurde nicht gestartet; keine automatischen Spieltests und keine Spielstand-/Profiländerungen.

Bitte nach vollständigem Neustart insbesondere bereits gespeicherte Snow Repeater laden und nach dem kurzen Shader-Ladevorgang ihre Originalgrafik prüfen. Danach eine neue Fusion und beide Handschuhvorschauen prüfen, einschließlich Leerlauf-/Schussanimation, Pause sowie erneutem Laden/Boardwechsel. Für Rückbau `backup/FusionMod-0.1.13.dll` nach `Mods/FusionMod.dll` kopieren.


## Korrektur 0.1.15: Katalogzugriff nach gemeldetem Absturz entfernt

Der Benutzer meldete einen Spielabsturz mit 0.1.14. Die gesicherten Logs enden nach der Board-Initialisierung, bevor die neue Meldung `Loading Snow Repeater shader through Addressables` erscheint. Es gibt darin keinen eindeutigen nativen Crash-Stacktrace; die genaue Absturzstelle ist deshalb nicht bewiesen. Die zeitliche Eingrenzung macht den in 0.1.14 eingeführten IL2CPP-Katalogzugriff verdächtig. Logs, DLL-Prüfsumme und ein Abbild des damaligen Darstellungsquellcodes sind unter `references/crash-0.1.14/` unverändert gesichert.

Der gesamte neue Katalogpfad ist entfernt: keine ResourceLocator-/Key-Iteration, keine typisierten generischen Shader-Anfragen, kein `Shader.Find` und keine eigenen Bundle-Ladevorgänge. Die Mod übernimmt stattdessen eine **eigene Materialkopie aus der nativen Pflanzenvorschau**. Bei der Handschuhvorschau stammt diese direkt aus ihrem PreviewController, ansonsten aus dem vorhandenen Cursor-PreviewController. Fehlt dieser, liefert dieselbe native Factory wie in LawnGlove eine vorübergehende versteckte Vorschau, die nach dem Kopieren wieder freigegeben wird. Fremde Cursorcontroller und deren Materialien werden weder verändert noch freigegeben. Die Materialkopie wird mit den übrigen eigenen Darstellungsassets aufgeräumt.

Damit wird das zur tatsächlichen Spielpipeline gehörende, bereits durch das Spiel geladene Sprite-Material verwendet, statt einen Shader über weitere IL2CPP-Generics aufzulösen. Die 17 Original-Sprites, Quell-Hierarchie, beiden Originalanimationen, Rezepte, Doppelschuss, Gefrierlogik und gespeicherte Variantenkennung bleiben erhalten. Der Aufbau wartet weiterhin auf `Playing`. Die Meldung `Snow Repeater uses native preview material` benennt das tatsächlich übernommene Material/den Shader; `Snow Repeater original art ready` hält die übrigen Darstellungswerte fest.

Release-Build: 0 Warnungen, 0 Fehler. Version 0.1.15, installierte Binärgleichheit und alle 18 eingebetteten Originalasset-Ressourcen sind statisch geprüft. Der entfernte Katalog-/Shaderladepfad wurde auch im dekompilierten Release-Build auf Abwesenheit geprüft. Vorversion: `backup/FusionMod-0.1.14.dll`; diese Vorversion ist die vom Benutzer als abgestürzt gemeldete Fassung und keine empfohlene Rückbauversion. Die älteren Sicherungen bleiben erhalten; LawnGlove ist unverändert.

Das Spiel wurde nicht gestartet. Es wurden keine Spielstände oder Profile verändert und keine automatischen Spieltests ausgeführt. **Crashfreiheit und sichtbare Darstellung sind noch nicht bestätigt.** Der nächste Benutzer-Spieltest sollte zuerst den gespeicherten Spielstand laden und prüfen, ob der Board-Start ohne Absturz gelingt; anschließend Originalgrafik/-animationen und beide Handschuhvorschauen prüfen.


## Korrektur 0.1.16: Skin-Anker und Größenberechnung

Das Benutzerlog von 0.1.15 bestätigt erfolgreich geladene Originalassets und das unterstützte Material `Sprites/DefaultColourblend`. Es zeigt zugleich native Mesh-Grenzen von rund 302 Millionen Einheiten und einen daraus berechneten Maßstab von 213.707.280. Diese Grenzen eignen sich nicht zur Größenbestimmung der Pflanzengrafik; die bisherige Anpassung skalierte und verschob das Artwork extrem.

`completecollection2_ef281/PvZReCoreLib.dll` zeigt in `SpriteRendererSkin.ApplySkin` und `SpriteRendererPlantSkin.ApplySkin` den zuständigen Anker: zusätzliche Sprites werden lokal an den CharacterSkinController gehängt, mit explizitem Maßstab. Die Fire-Peashooter-Assets verwenden 1 Pixel pro Einheit, einen mittigen Bodenpivot und unveränderte Prefab-Transformationen. Die FusionMod übernimmt dieses Ankerprinzip ohne neue DLL-Abhängigkeit. Sie behält ihre eigene SortingGroup bei, welche die native Sortierebene und Reihenfolge übernimmt und innerhalb der Pflanze die Originalreihenfolge erhält.

Die Originaldarstellung sitzt jetzt am Skin-Bodenanker. Ihre Proportionen werden in ein festes 80×80-Pflanzenfeld mit dem nativen Umrechnungsfaktor 2,82 eingepasst (225,6 Render-Einheiten). Handschuhvorschauen verwenden ausschließlich die echten `Sprite.bounds` ihrer nativen Vorschaugrafik. Native Spine-Mesh-Grenzen werden weder zum Warten noch für Maßstab oder Position verwendet. Grafik, Animationen und Rezepte bleiben erhalten. Nachweise, Referenzquellen und das vorherige Benutzerlog liegen unter `references/snow-repeater/rendering-0.1.16/`.

Release-Build: 0 Warnungen, 0 Fehler. Installierte DLL und Version sowie die 18 eingebetteten Originalassetdateien sind statisch geprüft. Vorversion: `backup/FusionMod-0.1.15.dll`. LawnGlove bleibt unverändert. Das Spiel wurde nicht gestartet; Sichtbarkeit und genaue Größen-/Bodenanpassung sind noch nicht im Spiel bestätigt. Bitte nach Neustart beide Rezepte, Leerlauf/Schussanimation, Handschuhaufnahme/Rastervorschau/Absetzen, Seerose/Topf/Kürbis und Speichern/Laden prüfen.


## Änderung 0.1.17: Snow Split Pea / Split Snow

**Split Snow** ist der Name in der Rezeptliste; die gewünschte **Snow Split Pea** entspricht der Originalpflanze `SnowSplit`, Quellen-ID 1107. Alle drei im lokalen Archiv registrierten Rezepte sind jetzt implementiert, jeweils in beiden Reihenfolgen:

```text
Ice-shroom | Split Pea | Split Snow
Peashooter | Snow Repeater | Split Snow
Repeater | Snow Pea | Split Snow
```

Alle Zutaten sind bereits native oder implementierte Pflanzen. Kartenfusion, Handschuhfusion und Ziel-Leuchten verwenden dieselbe zentrale Rezeptauflösung. Eine Karte bezahlt weiterhin die Kosten der eingehenden Zutat und verwendet deren Cooldown; beim Handschuh gelten die bestehenden Handschuhregeln. Varianten werden über `FusionPlant.From` erkannt. Insbesondere ergibt Peashooter auf Snow Repeater jetzt Split Snow; Peashooter auf Split Snow darf keine gewöhnliche Gatling Pea erzeugen. Gatling Snow bleibt offen.

### Verhalten und Speicherung

Eine Salve umfasst eine Eis-Erbse nach rechts und zwei gestaffelte Eis-Erbsen nach links: 20 × 3 Schaden je etwa 1,5 Sekunden gemäß lokalem Almanach. Native Split-Pea-KI und die vorhandene gekoppelte Zielsuche steuern beide Köpfe auch bei einem Ziel auf nur einer Seite. Die vorhandene Rückprallverwaltung dreht linke Schüsse am linken Bildschirmrand um. `SnowShooters.cs` wandelt Projektile für beide Eisvarianten vor ihrer nativen Initialisierung um; `SnowPea.cs` bleibt allein für Kälte, 10 Gefrierpunkte pro Treffer und vierfachen Schaden gegen eingefrorene Zombies verantwortlich. Torchwood verwendet weiterhin den tatsächlichen nativen Projektiltyp nach der Umwandlung.

Die sichere native Identität lautet `mSeedType = Splitpea` (28), `mImitaterType = Snowpea` (5). Dieselben bereits belegten Initialisierungs-, Serialisierungs-, Deserialisierungs- und Resetwege wie beim Snow Repeater erhalten die Variantenkennung, ohne das Speicherformat zu ändern. Der Controller und die Originaldarstellung werden beim Laden aus dieser Kennung wieder aufgebaut. Ohne diese Modversion erscheint die Pflanze als native Split Pea.

Bekannte Speichergrenzen bleiben bestehen: Gefrierpunkte und erhöhte Zombie-Schwellen beginnen nach Laden wieder bei 0/100. Bereits gespeicherte, rückwärts fliegende Projektile verlieren ihre Schützenzuordnung und damit ihren Rückprall; nach Laden neu abgefeuerte Projektile werden regulär erfasst. Bereits umgekehrte Projektile behalten ihre native Vorwärtsbewegung.

Der Originalalmanach nennt ein manuelles Klickupgrade nach 15 Sekunden zu Gatling Snow. Dieses Ergebnis ist noch nicht implementiert; das Upgrade bleibt gesperrt. Das bisher nicht portierte Klickupgrade des Snow Repeaters wird durch diese drei Rezepte ebenfalls nicht ergänzt. Beide Grenzen betreffen zusätzliche Transformationen außerhalb der ausgewählten Zweipflanzenrezepte.

### Originalassets und Nachweise

`references/snow-split/` enthält den lokalen Almanacheintrag 1107, das Almanachbild `SnowSplit` (Texture2D 2406), das Originalprefab und seine Darstellungskomponenten, statische DLL-Belege sowie Quellenprüfsummen. Die tatsächlichen Metadaten v31 und die CodeGenModule-Methodentabelle belegen `SnowSplit.GetBulletType` bei RVA `0x574b10`, `Shoot1` bei `0x574c90` und `Shoot2` bei `0x574f20`. Der reguläre Projektiltyp ist 15 (snowPea). Der Sonderfall mit einer weiteren, noch nicht implementierten Quellpflanze wird nicht vorweggenommen.

Aus dem lokalen `data.unity3d`, `resources.assets`, sind `SnowSplitPrefab` (GameObject 37188, Transform 120506), Controller 6363 sowie **20 Hierarchieobjekte, 16 Sprites und beide Animationsclips** eingebunden: `idle` (4335, 2 Sekunden) und `shoot` (4336, etwa 1,333 Sekunden). Originalpivots, Transformationen, Sichtbarkeitskanäle und kubische Animationskurven bleiben erhalten. Die Quellereignisse `AnimShoot`/`AnimShoot2` werden dokumentiert und nicht zusätzlich ausgeführt. Die native Schusslogik bleibt verantwortlich.

Der seit 0.1.16 bestätigte Renderweg ist jetzt für beide Eisvarianten in `src/ImportedPeaVisuals.cs` zusammengeführt. Jede Variante besitzt eigene eingebettete Modelle und GPU-Assets; gleiche Sprite-IDs können keine Modelle vermischen. Native Preview-Materialkopie, Skin-Anker, Sortierung, Schattenersatz, Handschuhvorschauen und Aufräumen werden gemeinsam verwaltet. Die Größenumrechnung basiert auf den nativen 80×80-Pflanzenabmessungen aus `PlantInitialize` und 2,82 Render-Einheiten je Boardpixel. Kopfanimationen berücksichtigen beim Split Snow beide nativen Kopfspuren. Genaues Aussehen und Phasengleichheit bleiben im Spiel zu prüfen.

Export und reine Offline-Assetvorschau unterstützen jetzt `--plant snow-split`:

```bash
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant snow-split
/tmp/pvz-fusion-extract-venv/bin/python tools/preview_snow_repeater.py --plant snow-split
```

Ohne Parameter bleibt der Export für Snow Repeater erhalten. Die Vorschau ist kein Spieltest. Das Quellenarchiv bleibt unverändert. Die drei erledigten Ergebniszeilen sind aus `fusionen.txt` entfernt; **655 offene Rezepte mit 540 Ergebnissen** bleiben erhalten, darunter alle noch offenen Rezepte mit Split Snow als Zutat.

### Build, Installation und manuelle Prüfung

Der erste Release-Build deckte ein vorhandenes Projektproblem auf: Dekompilierte `.cs`-Belege unter `references/` wurden durch das SDK automatisch mitkompiliert. `FusionMod.csproj` kompiliert jetzt ausdrücklich nur `src/**/*.cs`. Die Belege bleiben erhalten, zusätzliche fremde Bibliotheken sind nicht erforderlich.

Release-Build: **0 Warnungen, 0 Fehler**. Version **0.1.17** ist in Projekt, MelonInfo und README gleichgezogen. Installationsziel ist `Mods/FusionMod.dll` im oben genannten Replanted-Verzeichnis. Die vorherige installierte **0.1.16** ist unter `backup/FusionMod-0.1.16.dll` gesichert. Die neue DLL ist installiert; Binärgleichheit, alle 35 eingebetteten Assetdateien und Versionsmetadaten sind statisch geprüft (`references/snow-split/verification-0.1.17.json`); `SHA256SUMS` enthält die installierten DLL-Prüfsummen. LawnGlove wird nicht ersetzt. Rückbau: die gesicherte 0.1.16 als `Mods/FusionMod.dll` zurückkopieren. Eine gespeicherte Split-Snow-Pflanze verhält sich dann wie die normale Split Pea.

Das Spiel wurde nicht gestartet. Keine Spieltests oder automatischen Laufzeittests wurden ausgeführt; Spielstände und Profile bleiben unberührt. Bitte manuell prüfen:

- Alle drei Rezepte mit vertauschten Zutaten, Karten und beiden Handschuhrichtungen; genug/zu wenig Sonne, Cooldown, Abbruch und ungültige Felder.
- Ice-shroom als zweite Zutat sowie tagsüber schlafend als erste Zutat. Nachts kann eine zuerst gesetzte Ice-shroom vor dem zweiten Klick auslösen.
- Ein Zombie nur rechts, nur links und auf beiden Seiten: eine rechte und zwei linke Eis-Erbsen, linker Rückprall, Kälte/Gefrierpunkte, vierfacher Gefrierschaden und Torchwood-Konvertierung.
- Beide Originalköpfe, Leerlauf-/Schussanimation, Größe/Bodenlage, Sortierung, beide Handschuhvorschauen, Seerose, Dach/Topf und Kürbis. Snow Repeater als Regression mitprüfen.
- Speichern/Laden im Leerlauf und während einer Salve, danach neue Rückwärtsschüsse; Tod, Schaufel, Pool-Wiederverwendung und Boardwechsel ohne Grafikreste.
- Kein gewöhnliches Repeater-/Split-Pea-Rezept auf Eisvarianten; keine noch offene Gatling-Snow-Fusion oder Klicktransformation.


## Änderung 0.1.18: Split Snow zeigt native Split Pea

Der Benutzer meldet für 0.1.17 eine gewöhnliche Split-Pea-Darstellung. Das gesicherte MelonLoader-Log zeigt die erfolgreiche Splitpea-Fusion, aber keinen SnowSplit-Assetaufbau und keinen Ladefehler. Der statische Originalprefab-Nachweis erklärt den stillen Wartezustand: `SplitPea.m_skinController` ist leer. Der bisherige Renderweg wartete auf diesen optionalen Skin-Controller und kam deshalb nie bis zum Laden der Originalassets.

`ImportedPeaVisuals.cs` verwendet bei vorhandenem Skin-Controller weiterhin dessen Transform. Fehlt dieser, dient der native Mesh-Transform als Anker. Beim Originalprefab zeigen `m_renderOffset` und `m_meshRenderer` auf dasselbe Objekt `SplitPea_Render`; dessen Animationen und native Offsets bleiben erhalten. Shader, Originalassets und Größenumrechnung bleiben beim bestätigten Renderweg. Die hintere Schussanimation wird außerdem mit ihrem tatsächlichen Namen `splitpea_shooting` abgefragt, statt auf der hinteren Kopfspur erneut `shooting` zu suchen.

Belege: `references/snow-split/rendering-0.1.18/Latest-0.1.17.log` und `native-splitpea-prefab.json` mit Bundle-Prüfsumme. Version 0.1.18 erhöht Projekt, MelonInfo und README gemeinsam. Die vorher installierte 0.1.17 ist unter `backup/FusionMod-0.1.17.dll` gesichert; 0.1.18 ist installiert. Release-Build: 0 Warnungen, 0 Fehler. Versionsmetadaten und korrigierter Renderpfad sind im Release-Artefakt geprüft; installierte und gebaute DLL sind binärgleich (`rendering-0.1.18/verification.json`). Zum Rückbau diese Datei als `Mods/FusionMod.dll` zurückkopieren. Rezepte und offene Liste bleiben unverändert.

Nur Release-Build und statische Artefaktprüfung; das Spiel wird nicht gestartet. Der Benutzer prüft Sichtbarkeit beider Eisvarianten, beide Köpfe und Schussrichtungen, Handschuhvorschauen, Platzierung auf Topf/Seerose bzw. unter Kürbis und Laden einer gespeicherten Split Snow. Die bisherigen Speichergrenzen gelten weiter. Sichtbarkeit ist erst nach diesem Spieltest bestätigt.


## Änderung 0.1.19: Gemeinsames Wippen, additive Schussanimation und gleicher Maßstab

Der Benutzer bestätigt für 0.1.18 die Sichtbarkeit von Split Snow. Der Kopf blieb jedoch statisch, während der Stiel wippte; Split Snow war kleiner als Snow Repeater. Beim Snow Repeater löste sich der Kopf während des Schießens optisch vom Stiel. Die Quellenprüfung zeigt zwei gemeinsame Ursachen im Darstellungsadapter.

Beide Original-AnimatorController besitzen eine additive `Shoot`-Ebene (`m_LayerBlendingMode = 1`, Gewicht 1). Bisher wurden ihre Kopfpositions- und Skalenkurven absolut über die Leerlaufpose geschrieben. Damit verlor der Kopf während der Schussdarstellung die Leerlaufbewegung des Stiels. `ImportedPeaVisuals.cs` übernimmt jetzt die Originalkomposition: zuerst die vollständige Leerlaufpose, dann Positionsdifferenzen, Skalenverhältnisse und Rotationsdifferenzen des Schussclips gegenüber dessen Anfangspose. Sichtbarkeitskurven für Blinzeln bleiben diskret. Auch eine unveränderte native Schussphase kann den wippenden Kopf damit nicht mehr auf einer absoluten Position festhalten. Die Quelle hat beide Köpfe unter `PeaShooter_Head`; diese Hierarchie bleibt erhalten. Weder künstliche Kopfverbindungen noch zusätzliche Spielschüsse werden eingeführt.

Die Größe wurde bislang aus der gesamten sichtbaren Grafik pro Pflanze berechnet. Der breitere hintere Kopf führte dadurch zu einem kleineren Maßstab. Beide Originalprefabs besitzen denselben Bodenbereich: `Shadow`, Sprite 15757, 88×36 Pixel, PPU 100, lokaler Maßstab 7 und Wurzelmaßstab 0,23. Seine Breite beträgt 1,4168 Quelleinheiten. Beide Pflanzen verwenden nun diesen gemeinsamen Bezug und dessen Bodenposition statt der gesamten Kopfbreite. Bei der nativen Feldbreite 80×2,82 ergibt das für beide denselben lokalen Maßstab von etwa 159,2321. Split Snows hinterer Kopf erweitert die Silhouette nach links; Körper und vorderer Kopf werden nicht verkleinert. Der bisher sichtbare Snow Repeater bleibt damit die Größenreferenz. Handschuhvorschauen verwenden ebenfalls den gemeinsamen Quellbezug innerhalb der echten nativen Spritegrenzen.

Der Export hält die additive Ebene und den Bodenbezug in `model.json` sowie in `visual-assets.json` fest. Nicht unterstützte Ebenenmodi, Masken oder Nullreferenzskalen werden nicht stillschweigend übernommen. Die Offlinevorschau verwendet dieselbe additive Zusammensetzung und für beide Pflanzen dieselbe Kamera. Neue PNG-/GIF-Vorschauen unter `references/snow-repeater/` und `references/snow-split/` zeigen die Kopf-/Stielbewegung und den Schussrückstoß. Sie sind Assetprüfungen und keine Spieltests. Originalcontroller und Benutzerlog sind unter `references/animation-0.1.19/` gesichert.

Version 0.1.19 ersetzt ausschließlich FusionMod. Rezepte, Projektillogik, gespeicherte Variantenkennung und offene Rezeptliste bleiben unverändert. Die zuvor installierte 0.1.18 ist unter `backup/FusionMod-0.1.18.dll` gesichert. 0.1.19 ist installiert: Release-Build mit 0 Warnungen und 0 Fehlern, Versionsmetadaten und 35 eingebettete Assetdateien statisch geprüft, installierte und gebaute DLL binärgleich. Nachweis: `references/animation-0.1.19/verification.json`. Zum Rückbau diese Sicherung als `Mods/FusionMod.dll` zurückkopieren. Das Spiel wird nicht gestartet; Spielstände und Profile werden nicht verändert.

Manuell zu prüfen: beide Pflanzen nebeneinander im Leerlauf, Dauerfeuer und Schusspausen, Split Snows beide Schussrichtungen, Kopf-/Stielkontakt über die ganze Bewegung, gleicher Körper-/Kopfmaßstab, Handschuhaufnahme und beide Vorschauen, Dach/Topf/Seerose/Kürbis sowie Speichern/Laden. Die Sichtbarkeitsbestätigung für 0.1.18 bestätigt diese neuen Bewegungskorrekturen noch nicht.


## Änderung 0.1.20: Gatling Snow

Alle vier Originalrezepte sind implementiert. Jede Zutat ist bereits nativ oder als vollständige Mod-Variante vorhanden:

```text
Gatling Pea | Ice-shroom | Gatling Snow
Peashooter | Split Snow | Gatling Snow
Repeater | Snow Repeater | Gatling Snow
Snow Pea | Split Pea | Gatling Snow
```

Alle Rezepte gelten in beiden Reihenfolgen, über vorhandene Samenkarte oder Handschuh und dieselbe gemeinsame Zielmarkierung. Karten liefern weiterhin nur die jeweilige native Zutat. Snow Repeater und Split Snow werden als vorhandene Pflanzen oder am Handschuh erkannt; ihre Variantenkennung bleibt bei der Rezeptabfrage erhalten. Kosten und Karten-Cooldown gehören zur eingehenden Karte; der Handschuh verwendet seine bisherigen Kostenregeln. Terrain, Seerosen, Töpfe, Kürbisse und Erzeugung vor Zutatenverbrauch bleiben beim bestehenden Service.

Gatling Snow feuert vier gestaffelte Eis-Erbsen nach rechts je Salve, jeweils 20 Grundschaden, mit dem nativen Gatling-Salventakt von ungefähr 1,5 Sekunden. Gemeinsame Eis-Erbsentreffer verlangsamen, liefern 10 Gefrierpunkte und verursachen vierfachen Schaden gegen eingefrorene Zombies. Torchwood-Konvertierung bleibt nativ. Der native Gatling-Controller ist der KI- und Speicheradapter; die Eisprojektilauswahl erfolgt vor deren Initialisierung. Zusätzliche Salven oder Original-Animationsereignisse werden nicht ausgeführt.

Der lokale Almanach beschreibt einen Gatling-Schützen für Eis-Erbsen (20×4/1,5 Sekunden). Sein Begleittext erklärt, dass die heißen Läufe den sonst kalten Schützen vor dem Einfrieren bewahren. Originalbeschreibung, vier Rezeptregistrierungen, tatsächliche Methodenadressen und Binärbelege liegen unter `references/gatling-snow/`. Das vollständige Quellenarchiv bleibt unverändert.

Originalgrafik und Animationen stammen aus dem lokalen 4.0.5.2-Bundle: `SnowGatlingPrefab` (38426), Transform 107630, Controller 6294, Clips 4186/4187. 24 Transform-Knoten und 20 Sprites sind unter `assets/gatling-snow/` eingebettet. Die additive Schussebene mit Gewicht 1 verwendet dieselbe Komposition wie die bestehenden Eisvarianten. Der Bodenbezug hat dieselbe Breite 1,4168, aber eine andere Quellposition. Der Renderadapter normalisiert diese Position am belegten nativen `GatlingPea_Render`-Anker. Native Grafik wird erst nach erfolgreichem Aufbau ausgeblendet. Handschuh- und Rastervorschauen verwenden denselben Assetweg.

`FusionPlant.GatlingSnow` verwendet `Gatlingpea` (40) und `Snowpea` (5) als gespeicherte Variantenkennung. Laden baut die importierte Darstellung erneut auf. Die gemeinsame Bereinigung bei Tod, Pool-Reset, Controller- und Board-Ende gilt auch für diese Variante. Gefrierpunkte und Resistenzschwellen der Zombies werden weiterhin nicht gespeichert. Die bisher fehlenden Klickupgrades von Snow Repeater und Split Snow sind kein Bestandteil dieses Rezept-Ports; Gatling Snow wird über die vier Rezepte erzeugt.

Die vier erledigten Ergebniszeilen wurden aus `fusionen.txt` entfernt. Die Liste enthält jetzt 651 offene Rezepte mit 539 Ergebnis-Pflanzen.

Manuelle Benutzerprüfung: alle vier Rezepte in beiden Reihenfolgen, Kartenfusion soweit die eingehende Zutat als Karte vorhanden ist, beide Handschuhrichtungen, Zielmarkierung und unveränderte native Rezeptidentitäten. Außerdem vier Eis-Erbsen je Salve, Kälte/Gefrieren/Vierfachschaden, Torchwood, Kopf-/Stielkontakt und Größe, Handschuh-/Rastervorschau, Pause, Seerose/Topf/Kürbis sowie Speichern/Laden und Boardwechsel prüfen. Split Snow darf bei diesen Rezepten nicht als normale Split Pea gelten; Gatling Snow darf kein natives Gatling-Pea-Rezept auslösen.

Release-Build und Offline-Assetvorschau sind statische Nachweise. Das Spiel wurde nicht gestartet; Spieltests und Laufzeitbestätigung bleiben beim Benutzer. Vorherige installierte DLL: `backup/FusionMod-0.1.19.dll`. Zum Rückbau diese Sicherung nach `Mods/FusionMod.dll` kopieren. Vor dem Laden eines Spielstands mit Gatling Snow wieder Version 0.1.20 installieren, damit dessen Variantenkennung korrekt erkannt wird.


## Änderung 0.1.21: Solar-shooter

Das einzige registrierte Rezept für Solar-shooter (Quellen-ID 1000) ist vollständig über den gemeinsamen Fusionsweg verfügbar:

```text
Peashooter | Sunflower | Solar-shooter
```

Beide Zutaten sind nativ vorhanden. Beide Reihenfolgen funktionieren über Samenkarte und Handschuh; Zielmarkierung, Terrainprüfung, Zutatenverbrauch und Erhalt von Seerose, Topf und Kürbis verwenden den vorhandenen Service. Die eingehende Karte bestimmt Sonnenkosten und Cooldown; der Handschuh behält seine Kostenregeln. Das Ergebnis besitzt eine eigene Rezeptidentität und löst keine normalen Peashooter-Rezepte aus. Quellenarchiv und Glove-DLL bleiben unverändert. Die erledigte Ergebniszeile ist aus `fusionen.txt` entfernt; 650 Rezepte mit 538 Ergebnissen bleiben offen.

### Beschreibung und Verhalten

Solar-shooter produziert Sonne und verschießt Sonnenprojektile: 30 direkter Schaden pro Schuss bei etwa 1,5 Sekunden Abstand. Die Sonnenproduktion liefert 25 Sonne, erstmals nach 4–7 Sekunden, danach nach 25 Sekunden mit der originalen Zufallsabweichung von −2 bis +2 Sekunden. Sie benötigt kein Angriffsziel. Jeder dritte **abgefeuerte** Schuss lässt bei einem Zombietreffer eine einsammelbare Sonne im Wert von 5 fallen. Verfehlte Schüsse zählen mit, lassen jedoch keine Sonne fallen. Rüstung und Schilde verwenden weiterhin den nativen direkten Trefferpfad. Torchwood verändert diese Sonnenprojektile nicht; der originale TorchWood-Trigger wandelt nur gewöhnliche bzw. Eis-Erbsen um.

Die lokale Beschreibung erklärt, dass die Sonnenblume hinter dem Schützen seine Schwester ist: Sie erzeugt Sonne, während er sie mit Sonnenprojektilen beschützt. Maßgeblich sind der chinesische Almanach-Eintrag 1000 und die tatsächliche lokale 4.0.5.2-DLL, nicht das ältere inoffizielle Unity-Projekt. Belege und deutsche Beschreibung stehen unter `references/solar-shooter/`.

`SolarShooter.cs` ergänzt ausschließlich die Variante `Peashooter` + gespeicherter `Sunflower`-Marker. Native Zielsuche, Schussablauf und 300 Lebenspunkte stammen vom Peashooter. Ein eigener Produktionstimer vermeidet Konflikte mit dessen Schusszählern und pausiert beim Tragen, bei Pause und während Bungee-Zuständen. Produktionsaufhellung wird über den vorhandenen Farbpfad angenähert; der inkompatible Originalshader wird nicht geladen. Zusätzliche Original-Animationsereignisse erzeugen keine zweiten Schüsse.

Die Treffersonne ist ein nativer `SmallSun` mit individuell korrigiertem Wert. Replanted rechnet `SmallSun` normalerweise als 15. Ein Patch von `GetSunValue` allein genügt nicht: `Coin.ScoreCoin` enthält die Berechnung inline und ruft `Die` vor der Gutschrift auf. Ein synchroner, per Finalizer wiederhergestellter Kontext erfasst deshalb die Zuordnung vor dem Tod und setzt genau diese Gutschrift in `Board.AddSunMoney` auf 5. Andere Kleinsonnen bleiben bei 15. Manuelles Einsammeln und die vorhandene Sun-Collector-Mod verwenden denselben nativen Einsammelweg.

### Originalgrafik und Animationen

Der vorhandene Importweg übernimmt `PeaSunFlowerPrefab` 46095, Transform 117484 und Controller 6624: 24 Knoten, 18 Originalsprites, Leerlaufclip 4979 und Schussclip 4980. Die Schussebene ist additiv, Gewicht 1, ohne Maske. Material, Skin-Anker, Sortierung, Handschuh- und Rastervorschau sowie die Bereinigung bleiben gemeinsam verantwortlich. Die Bodenreferenz verwendet den tatsächlichen Solar-Shadow: Sprite 15757, Breite 0,88, lokaler Maßstab 7,4329915 und Wurzelmaßstab 0,23 ergeben 1,5044375 Quelleinheiten. Nicht ungeprüft den Eisvarianten-Maßstab übernehmen.

Der Original-Schussclip enthält zwei verwaiste Positions-/Skalenbindungen für Pfad-CRC32 `2349996250`. Dieser Transform fehlt im ausgelieferten Prefab; Unity kann die Bindungen ebenfalls nicht anwenden. Der Export protokolliert ausschließlich diese belegte Ausnahme in `visual-assets.json`; alle anderen unbekannten Bindungen bleiben Fehler. Der ähnlich benannte Tower-Controller 6654 gehört zu einem anderen Spielskript und wird nicht als Ersatz benutzt.

Auch die Sonnenprojektile verwenden Originalassets: `Bullet_normalSun` 63439, Controller 6142, Clip `Solar` 3765, vier Knoten und zwei Sprites. `ImportedSolarProjectileVisuals.cs` verwendet dieselben Modell-, Material- und Animationshelfer. Native Bewegung, Treffer und Speicherung bleiben beim Projektil. Native Grafik wird erst nach erfolgreichem Aufbau ausgeblendet und bei Fehlern wiederhergestellt. Alle benötigten Laufzeitassets sind in der DLL eingebettet.

Reproduzierbarer Export und Offlinevorschau:

```bash
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant solar-shooter
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant solar-projectile
/tmp/pvz-fusion-extract-venv/bin/python tools/preview_snow_repeater.py --plant solar-shooter
```

### Speicherung und Benutzerprüfung

Die Pflanzenidentität bleibt über die nativen Typ-/Markerfelder gespeichert; Laden erzeugt wieder Solar-shooter mit Originaldarstellung. **Zusatzdaten werden bislang nicht gespeichert:** Produktionstimer, Aufhellung und Dreierschusszähler beginnen nach Laden neu. Bereits fliegende Sonnenprojektile werden nach Laden zu gewöhnlichen nativen Erbsen mit deren Grafik und Schaden, ohne Sonnenbonus. Noch nicht gutgeschriebene 5-Sonnen-Pickups verlieren nach Laden ihre Zuordnung und zählen dann nativ 15. Vor Speichern daher alle Treffersonnen vollständig einsammeln und laufende Schüsse abwarten. Diese Grenze gilt auch während der Einsammelanimation. Native Speicherformate, Profile und bestehende Spielstände werden nicht durch zusätzliche Felder oder eigene Schreibzugriffe verändert.

Manuell prüfen: beide Kartenreihenfolgen und Handschuhrichtungen; Kosten, Cooldowns, Zielmarkierung, Abbruch und ungültige Felder; 30 Schaden, jeder dritte Schuss mit 5-Sonnen-Trefferbonus, Fehlschüsse, Schilde und Torchwood; 25-Sonnen-Produktion ohne Ziel, Pause und Tragen; Originaldarstellung, Kopfkontakt, Aufhellung, Mündungsposition und Projektilgröße; Handschuh-/Rastervorschau, Seerose/Topf/Kürbis; Speichern/Laden, Boardwechsel und bestehende Eisvarianten. Normale Peashooter und Sunflower müssen ihre bisherigen Rezepte behalten.

Release-Build: 0 Warnungen, 0 Fehler. Pflanzen-Offlinevorschau visuell geprüft; Build-Ressourcen und installierte DLL statisch geprüft (siehe `references/solar-shooter/verification-0.1.21.json`). Das Spiel wurde nicht gestartet; Spieltests bleiben vollständig beim Benutzer. Vorversion/Rückbau: `backup/FusionMod-0.1.20.dll` nach `Mods/FusionMod.dll` kopieren. Vor dem Laden eines Spielstands mit Solar-shooter wieder mindestens Version 0.1.21 installieren.


## Änderung 0.1.22: Solar-Projektile am nativen Initialisierungspunkt registrieren

Der Benutzer meldet zu 0.1.21 gewöhnliche Erbsen statt Sonnenprojektilen. Das gesicherte Log unter `references/solar-shooter/shooting-0.1.22/Latest-0.1.21.log` zeigt den erfolgreichen Aufbau der Solar-Pflanze, aber weder Materialaufbau noch einen Fehler der Sonnenprojektilgrafik. Die bisherige Erfassung über Rückgabewerte der beiden `Board.AddProjectile`-Überladungen bietet damit keinen Laufzeitnachweis einer erfolgreichen Zuordnung.

Die Zuordnung erfolgt jetzt direkt nach `Projectile.ProjectileInitialize`. Statisch bestätigt: Beide nativen `AddProjectile`-Überladungen setzen App und Board und rufen denselben Initialisierer (`0x6c8e60`) auf. Nach dessen Rückkehr sind Typ, Board und Projektilzustand initialisiert. Der gemeinsame synchrone Kontext umfasst `Plant.UpdateShooting` und direkte `Plant.Fire`-Aufrufe; verschachtelte Aufrufe stellen den vorherigen Schützen per Finalizer wieder her. Der alte Solar-Patch auf den Überladungsrückgaben entfällt. Gewöhnliche Erbsenschützen ohne Solar-Marker erhalten keine Solar-Zuordnung.

Die ersten Kontext- und Registrierungsmeldungen werden einmal je Solar-Pflanze ausgegeben, der erfolgreiche Originalgrafikaufbau einmal je Board. Damit lässt sich bei der Benutzerprüfung zwischen fehlendem Schützenkontext, fehlender Projektilzuordnung und fehlender Darstellung unterscheiden. Diagnosezustand wird bei Reset, Tod, Board- und Mod-Ende bereinigt. Originalassets, 30 Schaden, Dreierschussbonus, Produktion, Rezepte und die in 0.1.21 dokumentierten Speichergrenzen bleiben unverändert.

Release-Build: 0 Warnungen, 0 Fehler. Version und eingebettete Originalassets statisch geprüft, Build und installierte DLL binärgleich. Vorversion: `backup/FusionMod-0.1.21.dll`; Rückbau durch Kopieren nach `Mods/FusionMod.dll`. Das Spiel wurde nicht gestartet. Nach vollständigem Neustart selbst prüfen: sichtbare Sonnenprojektile, 30 Schaden, 5 Sonne beim Treffer jedes dritten Schusses, gewöhnliche Erbsenschützen und Eisvarianten. Die Laufzeitursache ist ohne diese Benutzerprüfung nicht abschließend bestätigt.


## Änderung 0.1.23: Richtigen Peashooter-Projektiltyp akzeptieren

Der erneute Benutzerbefund zu 0.1.22 und dessen gesichertes Log grenzen den Fehler auf die Registrierung ein: Der Solar-Schützenkontext wird erreicht, die Projektilregistrierung jedoch nicht. Die tatsächliche Ursache liegt im Typfilter von `SolarShooter.Track`: Replanted erzeugt für die native Peashooter-Pflanze `ProjectileType.PeashooterPea` (14), während der Filter ausschließlich `ProjectileType.Pea` (0) akzeptierte. Damit wurde jedes Solar-Projektil vor der Zuordnung verworfen; Schaden, Dreierschussbonus und Originaldarstellung erhielten ebenfalls keine Zuordnung. Das Verschieben des Registrierungspunkts in 0.1.22 konnte diesen Filterfehler nicht beheben.

Statischer Beleg: `Plant.Fire` prüft den Pflanzentyp bei RVA `0x6b06e7` auf 0 und setzt bei `0x6b06eb` den Projektiltyp auf `0x0e`. Bei `0x6b0cc8` übergibt es diesen Wert an `Board.AddProjectile`, Aufruf bei `0x6b0cd4`. Die Enum-Werte aus der tatsächlich installierten Interop-Assembly sind unter `references/solar-shooter/shooting-0.1.23/` festgehalten.

Der Filter akzeptiert jetzt die beiden gewöhnlichen Erbsentypen `Pea` und `PeashooterPea`, weiterhin ausschließlich im synchronen Schusskontext einer Solar-Pflanze auf demselben Board. Die zusätzliche äußere `UpdateShooting`-Abfangung aus 0.1.22 ist entfernt; der statisch belegte direkte `Plant.Fire`-Kontext genügt. Der gemeinsame Initialisierungspunkt, Dreierschusszähler, Lebenszyklus und Originalassets bleiben erhalten. Diagnosemeldungen geben nun numerische DataIDs aus. Normale Peashooter werden durch ihren fehlenden Solar-Marker nicht verändert.

Release-Build: 0 Warnungen, 0 Fehler. Version, Typfilter und eingebettete Assets statisch geprüft; installierte und gebaute DLL binärgleich. Vorversion ist `backup/FusionMod-0.1.22.dll`; Rückbau durch Kopieren nach `Mods/FusionMod.dll`. Spiel nicht gestartet, keine Spieltests ausgeführt. Benutzerprüfung nach Neustart: Sonnenprojektile, 30 Schaden, jeder dritte Schuss mit 5-Sonnen-Trefferbonus, Torchwood sowie normale Peashooter. Die bekannten Speichergrenzen bleiben bestehen.


## Änderung 0.1.24: Cherryshooter

Das einzige Quellenrezept für Cherryshooter (ID 1001) ist vollständig registriert:

```text
Cherry Bomb | Peashooter | Cherryshooter
```

Beide Zutaten sind native, verwendbare Replanted-Pflanzen. Beide Reihenfolgen funktionieren über Samenkarte und Handschuh; Zielmarkierungen verwenden dieselbe zentrale Prüfung. Kartenkosten und Karten-Cooldown gehören zur eingehenden Zutat (gewöhnlich Cherry Bomb: 150 Sonne, Peashooter: 100 Sonne). Beim Handschuh gelten die bisherigen Kosten und der bisherige Cooldown. Ergebnis entsteht vor dem Zutatenverbrauch. Seerose, Topf und Kürbis bleiben erhalten.

Cherryshooter verschießt rosa Kirschprojektile mit **40 Direktschaden bei etwa 1,5 Sekunden Schussabstand**. Zielwahl, Schussabstand und Kollision übernimmt der native Peashooter; nur Projektile aus dem synchronen Cherryshooter-Schusskontext erhalten Kirschverhalten. Gewöhnliches Torchwood verwandelt diese Projektile nicht. Die normale Kirschkugel verursacht keine Flächenexplosion. Beschreibung und Werte stammen aus dem lokalen 4.0.5.2-Almanach. Der Beschreibungstext schildert Cherryshooters Abneigung gegen Witze über seinen roten Hinterkopf und seine besonderen Kirschkugeln. Quellen-RVAs, Almanach und Assets sind unter `references/cherry-shooter/` dokumentiert.

Die Pflanzenidentität ist `Peashooter` + `Cherrybomb` in `mImitaterType`. Sie bleibt von normalen Peashootern und Solar-shootern getrennt, auch als Handschuhzutat und nach Laden. Das vorhandene native Feld wird gespeichert; keine fremden IDs oder sachfremden Zähler werden verwendet. Das Ergebnis löst daher keine Rezepte für einen normalen Peashooter aus. Weiterentwicklungen mit Cherryshooter als Zutat bleiben offen.

Originalgrafik und Originalanimationen aus `data.unity3d` sind eingebettet: **CherryshooterPrefab 37187**, Transform 120505, Controller 6593, Clips 4907 (`idle`) und 4908 (`shoot`), 21 Knoten und 19 Sprites. Die Schussebene ist additiv, Gewicht 1, ohne Maske. Das ähnlich benannte **CherryShooterPrefab 47422 gehört zur Tower-Klasse und wird nicht verwendet**. Originalprojektil `Bullet_cherry` 61266 enthält drei Knoten und zwei Sprites, aber keinen Animator. Es bleibt originalgetreu statisch; das leere Laufzeit-Zeitfenster `idle` enthält keine erfundene Animation. Pflanze und Projektil verwenden den belegten Schattenbezug 0,88 × 7 × 0,23 = 1,4168 Quelleinheiten. Material, Vorschauen, Sortierung und additive Animation verwenden den bestehenden Importweg. Solar- und Kirschprojektile teilen sich jetzt dessen Darstellungsadapter; die Schadens- und Produktionsregeln bleiben bei ihren Pflanzen. Treffergeräusche und Aufpralleffekte stammen weiterhin aus Replanted.

Speichergrenze: Bereits fliegende Kirschprojektile verlieren beim Laden ihre flüchtige Zuordnung, Originaldarstellung, 40 Schaden und Torchwood-Sperre; sie werden zu gewöhnlichen nativen Erbsen. Neu abgefeuerte Projektile erhalten Kirschverhalten wieder. Pflanzenidentität und Pflanzenanimation werden aus den nativen Feldern wiederhergestellt. Die zusätzliche zufällige Explosion in `Bullet_cherry.HitZombie` ist ausdrücklich an ein Fusion-Reiseupgrade gebunden; dieses fremde Modussystem gehört nicht zur normalen Cherryshooter-Fusion und wird nicht portiert.

Der erledigte Ergebnis-Eintrag ist aus `fusionen.txt` entfernt; das vollständige Quellenarchiv bleibt unverändert. Offen sind jetzt 649 Rezepte mit 537 Ergebnissen. Release-Build: 0 Warnungen, 0 Fehler. Originalassets wurden offline exportiert und die additive Animationsvorschau visuell geprüft. Das bestätigt keine Darstellung oder Funktion im Spiel. Spiel nicht gestartet; keine Spieltests, Spielstand- oder Profiländerungen. Vorversion: `backup/FusionMod-0.1.23.dll`. Rückbau durch Kopieren dieser DLL nach `Mods/FusionMod.dll`; vor Laden eines Spielstands mit Cherryshooter wieder eine passende Mod-Version installieren.

Manuelle Benutzerprüfung:

- Beide Kartenreihenfolgen; Cherry Bomb sofort auf vorhandenen Peashooter setzen. Eine allein gepflanzte Cherry Bomb darf weiterhin normal explodieren.
- Beide Handschuhrichtungen, soweit Cherry Bomb vor der nativen Explosion aufgenommen werden kann; deren Zünder nicht für bequemere Fusion verzögern. Abbruch, ungültige Ziele und Zielmarkierung prüfen.
- Kartenkosten/Cooldown, Handschuh-Cooldown, Seerose/Topf/Kürbis und Zutatenverbrauch prüfen.
- Pflanzenanimation, Vorschauen, Größe/Bodenlage, Mündungsposition, rosa Projektile, 40 Direktschaden, Schussabstand und Torchwood prüfen. Gewöhnliche Peashooter sowie Solar- und Eispflanzen gegenprüfen.
- Pause, Tragen, Tod, Boardwechsel und Speichern/Laden prüfen; fliegende Kirschprojektile haben die beschriebene Speichergrenze.


## Änderung 0.1.25: Explode-o-shooter

Das einzige Quellenrezept für Ergebnis-ID 1005 ist vollständig implementiert:

```text
Cherry Bomb | Cherryshooter | Explode-o-shooter
```

Cherryshooter ist seit 0.1.24 implementiert; Cherry Bomb ist nativ verfügbar. Beide Reihenfolgen gelten über die gemeinsame Rezeptauflösung und den Handschuh. Die Samenkarte erlaubt Cherry Bomb auf Cherryshooter. Eine Cherryshooter-Samenkarte wird nicht erfunden. Die Gegenrichtung verwendet den Handschuh und eine noch nicht explodierte Cherry Bomb; deren Zünder wird nicht verzögert. Kartenkosten und Handschuh-Cooldown bleiben nativ. Gemeinsame Terrainprüfung, Zielmarkierung, Erzeugung vor Zutatenverbrauch und Erhalt von Seerosen, Töpfen und Kürbissen bleiben bestehen.

### Beschreibung und Verhalten

Der lokale Original-Almanach beschreibt einen äußerst reizbaren Kirschschützen, der die in seinem Hals steckende Kirsche tatsächlich abschießen könnte; sein rotes Gesicht verrät die Anstrengung. Er verschießt **alle 3 Sekunden** explodierende Kirschen mit **300 Flächenschaden**, **kreisförmigem Radius eines Feldes** und **ohne Entfernungsabfall**. Die Portierung verwendet native Zombie-Berechtigungen und Trefferrechtecke, einen Kreisradius von 80 Board-Pixeln sowie höchstens eine benachbarte Reihe. Der Primärtreffer erhält keinen zusätzlichen Erbsenschaden. Native Explosionsflags behandeln Schilde und Körper. Die native Großexplosion wird vermieden, da sie zusätzlich Leitern löscht. Normales Torchwood verwandelt die Projektile nicht.

Die für diese erfolgreiche Fusion verbrauchte **Cherry-Bomb-Karte erhält 85 % ihres Cooldowns sofort gutgeschrieben**: normalerweise verbleiben etwa **7,5 statt 50 Sekunden**. Der native Nachladezähler speichert die Gutschrift. Die normale Kartendauer bleibt unverändert; nachfolgende gewöhnliche Cherry-Bomb-Nutzungen laden normal nach. Abbruch, ungültige Platzierung, Cherryshooter-Erzeugung und Handschuhfusion erhalten keinen Kartenbonus. Das erfolgreich erzeugte Ergebnis wird im synchronen `PlayerPlantingScope` erfasst, bevor das Spiel `SeedPacket.WasPlanted` aufruft.

Der Almanach nennt außerdem Immunität gegen Kirsch-Explosionsschaden. Freundliche Cherry Bombs und diese Projektile schädigen in Replanted ohnehin keine Pflanzen. Keine allgemeine Unverwundbarkeit gegen Zombieangriffe, Jack-in-the-Box oder Katapultschaden hinzufügen. Die feindlichen Kirschsysteme und Reiseupgrades des Quellspiels werden nicht als zusätzliche Systeme eingeführt.

### Originalassets und Speicherung

Originalpflanze aus `data.unity3d`: **SuperCherryShooterPrefab 47421**, Transform 105493, Script `globalgamemanagers.assets:690 SuperCherryShooter`, Controller 6637, Clips 5022 (`idle`, 2 Sekunden) und 5023 (`shoot`, 1 Sekunde), **12 Knoten und 11 Sprites**. Schussebene: additiv, Gewicht 1, ohne Maske. Im Leerlaufclip fehlen drei Transformziele bereits im ausgelieferten Prefab. Ausschließlich diese belegten Positionsbindungen werden wie in Unity ignoriert und im Exportmanifest dokumentiert; andere unbekannte Bindungen bleiben Fehler.

Originalprojektil **Bullet_superCherry 61265**, Transform 104370, Script `globalgamemanagers.assets:2218 Bullet_superCherry`: **drei Knoten, zwei Sprites, kein Animator**. Originalpivots, Hierarchie, Sortierung und Animationen verwenden den vorhandenen Importweg, einschließlich Handschuh- und Rastervorschau. Gemessene Schattenbreite: 0,88 × 6,70833397 × 0,24 = **1,4168 Quelleinheiten**. Pflanzen und Projektile verwenden diesen Größenbezug. Alle Laufzeitassets sind eingebettet. **Asset-Grenze:** Explosionsblitz (`Powie`) und Geräusch (`Cherrybomb`) stammen aus Replanted; Quell-Partikel und Quell-Audiodaten werden nicht geladen.

Die reservierte Speicherkennung lautet **Peashooter + ExplodeONut (49)** im nativen `mImitaterType`. Diese gültige Enum-Kennung ist weder Imitater-Marker 48 noch eine Quellen-ID. Das Paar darf nicht für andere Fusionen verwendet werden. `FusionPlant.From` trennt Explode-o-shooter von Peashooter, Cherryshooter und Solar-shooter. Pflanzenidentität, Schussintervall und laufender Schusszähler verwenden die vorhandenen nativen Speicherfelder. Laden stellt die Originaldarstellung wieder her.

**Speichergrenze:** Bereits fliegende Explosivkirschen verlieren beim Laden ihre flüchtige Zuordnung, Originalgrafik, Explosion und Torchwood-Sperre; sie werden gewöhnliche native Erbsen. Neue Schüsse erhalten wieder das korrekte Verhalten. Vor Speichern laufende Schüsse abwarten. Tod, Pool-Reset, Board-Ende und Mod-Ende verwenden die gemeinsamen Aufräumwege.

Belege, übersetzte Beschreibung, Original-Almanach, Methodenauszüge, Exportmanifest und Offlinevorschau liegen unter `references/explode-o-shooter/`. Reproduzierbare Werkzeuge:

```bash
/tmp/pvz-fusion-extract-venv/bin/python tools/research_explode_o_shooter.py
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant explode-o-shooter
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant explode-projectile
/tmp/pvz-fusion-extract-venv/bin/python tools/preview_snow_repeater.py --plant explode-o-shooter
```

Der erledigte Ergebnis-Eintrag ist aus `fusionen.txt` entfernt. Offen bleiben **648 Rezepte mit 536 Ergebnissen**. Die Kopfzeile zählt dabei nicht als Rezept. `fusionen-quellen.json` bleibt unverändert; Gatling Cherrybomber als Folgeergebnis bleibt offen.

Release-Build: 0 Warnungen, 0 Fehler. Originalanimation offline visuell geprüft; Version, eingebettete Assets und Binärgleichheit der Installation statisch geprüft. Kein Spielstart, keine Spieltests, keine Spielstand- oder Profiländerungen. Vorversion/Rückbau: `backup/FusionMod-0.1.24.dll` nach `Mods/FusionMod.dll` kopieren. Vor Laden eines Spielstands mit Explode-o-shooter wieder mindestens 0.1.25 verwenden. LawnGlove bleibt unverändert bei 1.2.2.

Manuelle Benutzerprüfung:

- Cherry Bomb auf Cherryshooter; beide Handschuhrichtungen im nativen Zündfenster. Abbruch, ungültige Ziele, Zielmarkierung und Erhalt der Zutaten bei gescheiterter Platzierung prüfen.
- Kosten und etwa 7,5 Sekunden Rest-Cooldown nach erfolgreicher Kartenfusion prüfen. Ohne Fusion sowie bei Cherryshooter-Erzeugung muss Cherry Bomb normal nachladen; Handschuh behält seinen eigenen Cooldown.
- 300 Flächenschaden, Kreisrand, mehrere Zombies, benachbarte Reihen, Rüstung/Schilde, fehlenden zusätzlichen Direkttreffer und drei Sekunden Schussabstand prüfen. Torchwood darf keine Feuererbse erzeugen.
- Originalgrafik, Animationen, Mündungsposition, Größe/Bodenlage, beide Vorschauen, Explosionsblitz und Seerose/Topf/Kürbis prüfen.
- Pause, Tragen, Bungee, Tod, Boardwechsel und Speichern/Laden prüfen; fliegende Projektile haben die genannte Speichergrenze. Normale Cherry Bombs, Cherryshooter, Solar-shooter und Eisvarianten gegenprüfen.


## Änderung 0.1.26: Sichtbare Fusionsmarkierung auf importierter Grafik

Cherryshooter soll beim Halten einer Cherry-Bomb-Karte als gültiges Ziel für Explode-o-shooter leuchten. Das Rezept und seine zentrale Zielprüfung waren bereits vorhanden. Der Fehler lag in der importierten Darstellung: Der native Farbupdate setzt die additive Hervorhebungsfarbe, während `ImportedPeaVisuals` bisher nur den weißen Multiplikationsfarbwert gegen Weiß interpolierte. `Lerp(white, white, Stärke)` ändert die Texturfarbe nicht; die sichtbare Originalgrafik erhielt keine Hervorhebung.

Der gemeinsame Pflanzenrenderer übernimmt additive Farben jetzt über die vorhandenen Eigenschaften `_BlendColor` und `_BlendStrength` des bereits verwendeten nativen Shaders `Sprites/DefaultColourblend`. Die vorhandene Variante `_BLEND_ON` wird ausschließlich auf der eigenen Pflanzenmaterialkopie aktiviert. Pro Renderer hält ein `MaterialPropertyBlock` die aktuelle Farbe und Stärke; andere Pflanzen mit demselben Material bleiben unabhängig. Stärke wird bei jedem Darstellungsupdate neu gesetzt und bei deaktiviertem Effekt auf null zurückgesetzt. Der Originalschatten bleibt unverändert. Native Hover-Markierungen und Schadensaufhellung verwenden denselben Weg. Rezeptauflösung, Zielprüfung, Kosten und Spielverhalten bleiben unverändert; es gibt keine zusätzliche Cherryshooter-Sonderprüfung.

Statische Belege liegen unter `references/highlight-0.1.26/`: `Plant.UpdateReanimColor` ab RVA `0x6bb0f0` setzt bei `mHighlighted` über `ReloadedController.SetExtraAdditiveColor` (`0x7cad50`) Weiß mit Alpha `196/255`. Der bisherige Importpfad konnte diese Aufhellung nicht darstellen. Das Shaderarchiv belegt Farbblend-Eigenschaften und Keyword; das gesicherte Benutzerlog belegt die importierte Darstellung und erfolgreiche Cherryshooter-Folgefusionen in 0.1.25.

Release-Build: 0 Warnungen, 0 Fehler. Version und installierte DLL statisch geprüft; Installation und Build binärgleich. Vorversion/Rückbau: `backup/FusionMod-0.1.25.dll` nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert. Spiel nicht gestartet, keine Spieltests, keine Änderungen an Spielständen oder Profilen.

Manuell prüfen: Cherry-Bomb-Karte halten, mehrere Cherryshooter als Ziele ansehen, Karte abbrechen und Werkzeug wechseln. Gültige Cherryshooter sollen sichtbar aufhellen; ungeeignete Solar-shooter und Explode-o-shooter dürfen keine Fusionsmarkierung erhalten. Cherry Bomb am Handschuh, normalen Hover, Schadensaufhellung, Pause und andere importierte Pflanzen gegenprüfen. Eine Aufhellung darf nicht auf andere Pflanzen mit gemeinsamem Material übergreifen.


## Änderung 0.1.27: Sanftere Hervorhebung

Die Hervorhebung importierter Pflanzen verwendet jetzt die halbe bisherige Blendstärke: bei der normalen weißen Markierung etwa 0,384 statt 0,769. Das erhält mehr Originalfarbe und Details. Fusionsziele und normale Hover-Markierungen verwenden dieselbe Dämpfung; Schadensaufhellung ohne gleichzeitige Markierung bleibt unverändert.

`FusionHighlightPatch` erfasst die Markierung nach dem nativen Farbupdate, bevor sein Finalizer das temporäre Fusionsflag zurücksetzt. Die bereits vorhandene Darstellungsinstanz hält diesen kosmetischen Zustand bis zum nächsten Farbupdate. Kein zusätzlicher Rezeptcheck oder separater globaler Zustand ist nötig. Vorschauen, Schatten und Rezeptverhalten bleiben unverändert.

Release-Build: 0 Warnungen, 0 Fehler. Version und Binärgleichheit statisch geprüft. Vorversion/Rückbau: `backup/FusionMod-0.1.26.dll` nach `Mods/FusionMod.dll` kopieren. Spiel nicht gestartet; die Benutzerprüfung umfasst Helligkeit auf Cherryshooter, Abbruch/Werkzeugwechsel und unveränderte Schadensaufhellung.


## Änderung 0.1.28: Cherrypeater und Quellenautomatismus

Alle zwei Quellenrezepte für Ergebnis-ID 1008 sind implementiert, jeweils in beiden Reihenfolgen:

```text
Cherry Bomb | Repeater | Cherrypeater
Cherryshooter | Peashooter | Cherrypeater
```

Beide Zutatenpaare sind bereits verfügbar. Kartenfusionen verwenden die vorhandenen nativen Zutatenkarten; Cherryshooter wird als vorhandene Pflanze erkannt. Der Handschuh unterstützt beide Richtungen. Gemeinsame Zielprüfung, Hervorhebung, Terrainprüfung und Erzeugung vor Zutatenverbrauch bleiben in den bestehenden Verantwortlichkeiten.

Der lokale 4.0.5.2-Almanach beschreibt zwei rosa Kirschkugeln mit je 40 Direktschaden pro Salve bei 1,5 Sekunden Quellenintervall. Cherrypeater verwendet den nativen Repeater für Zielsuche, Doppelschuss, Schusszähler und Kollision. Dessen native Taktung einschließlich zufälliger Verkürzung um 0–14 Simulationsticks bleibt erhalten. Beide Kugeln verwenden den vorhandenen Cherryshooter-Trefferpfad: 40 Direktschaden, keine Flächenexplosion und keine Torchwood-Konvertierung. Das beschriebene Klickupgrade nach 15 Sekunden erzeugt Split Cherry; dieses noch offene Ergebnis und seine Rezepte bleiben ausgespart.

Originaldarstellung: `DoubleCherryPrefab` 37191, Transform 120509, Script `globalgamemanagers.assets:2357 DoubleCherry`, Animator 6596, Clips 4913/4914. Alle 22 Knoten, 20 Sprites und beide Animationen stammen aus dem lokalen Originalbundle. Die additive Schussebene und Leerlaufgeschwindigkeit 1,3 werden übernommen. Beide Original-Schussereignisse sind Belege; Projektile erzeugt ausschließlich die native KI. Die Originalkirschkugel 61266 wird aus der bestehenden Ressource wiederverwendet. Die Bodenbreite beträgt 0,88 × 7 × 0,23 = 1,4168 Quelleinheiten. Herkunft, Beschreibung, Rezepte, Disassembly und Offlinevorschau liegen unter `references/cherrypeater/`.

Die persistierte Identität ist `Repeater` + `Cherrybomb` in `mImitaterType`. `FusionPlant.From` verhindert gewöhnliche Repeater-Folgefusionen. Native Pflanzenfelder speichern diese Kennung; die Mod baut Grafik und Schussart nach Laden wieder auf. Bereits fliegende Kirschkugeln verlieren beim Laden ihre flüchtige Sonderzuordnung und sind dann native Erbsen. Für Spielstände mit Cherrypeater ist die passende Mod-Version erforderlich.

### Wiederverwendbare Quellenrecherche

`tools/research_fusion.py` ersetzt das Kopieren und Anpassen pflanzenspezifischer Rechercheprogramme für weitere Fusionen. Ein Aufruf archiviert alle Quellenrezepte eines Ergebnisnamens, den lokalen Almanacheintrag, die tatsächlichen Methodenadressen und vollständige Methodenauszüge ausgewählter Klassen. Mit `--prefab` sichert er zusätzlich die Wurzelkomponenten, die über Datei-ID aufgelöste Scriptidentität sowie Animator und Clip-IDs. Eingabeprüfsummen belegen die Quellen. Die DLL-Prüfsumme muss zum Quellenarchiv passen; eine andere Version erfordert eine erneute Prüfung der festgelegten Metadatenoffsets und Methodentabelle. Unbekannte oder mehrdeutige Klassen brechen ab. Methodentokens im Dateinamen verhindern das Überschreiben gleichnamiger Überladungen.

Abhängigkeiten: Python mit UnityPy 1.25.4, Pillow, pefile 2024.8.26 und capstone 5.0.9. Eine Umgebung kann beispielsweise unter `/tmp/pvz-fusion-research-venv` erstellt werden. Temporäre Umgebungen sind nicht dauerhaft vorhanden und müssen bei Bedarf neu eingerichtet werden.

```bash
python3 -m venv /tmp/pvz-fusion-research-venv
/tmp/pvz-fusion-research-venv/bin/pip install UnityPy==1.25.4 Pillow pefile==2024.8.26 capstone==5.0.9
/tmp/pvz-fusion-research-venv/bin/python tools/research_fusion.py --result Cherrypeater --slug cherrypeater --type DoubleCherry --type Shooter --type Bullet_cherry --prefab 37191
/tmp/pvz-fusion-research-venv/bin/python tools/export_snow_repeater.py --plant cherrypeater
/tmp/pvz-fusion-research-venv/bin/python tools/preview_snow_repeater.py --plant cherrypeater
```

Der Automatismus wurde an Cherrypeater ausgeführt: zwei Rezepte, Almanach-ID 1008, 18 Methoden und Script DoubleCherry korrekt erfasst. Er beurteilt weder Zutatenverfügbarkeit noch Verhaltensgleichheit automatisch. Diese Entscheidungen und die Registrierung des Assetexports bleiben Teil des Workflows.

Release-Build: 0 Warnungen, 0 Fehler. Offlinevorschau visuell geprüft; eingebettete Cherrypeater-Ressourcen mit den Exportdateien verglichen. Kein Spielstart, keine Spieltests und keine Änderungen an Profilen oder Spielständen.

Manuelle Prüfung durch den Benutzer: beide Rezepte und beide Handschuhrichtungen, Cherry Bomb vor Ablauf ihres nativen Zünders, Kartenkosten und Cooldown, gültige und ungültige Hervorhebung, zwei Kirschkugeln mit je 40 Schaden, Torchwood, Originalanimation und Mündungsposition, Pause/Spielgeschwindigkeit, Seerose/Topf/Kürbis, Speichern/Laden sowie Abgrenzung zu Repeater und Snow Repeater. Split-Cherry-Klickupgrade wird nicht angeboten.

Installation: Version 0.1.28 liegt unter `Mods/FusionMod.dll`; Binärgleichheit und Versionsattribute wurden statisch geprüft. Die vorherige Version liegt unverändert unter `backup/FusionMod-0.1.27.dll`. Für den Rückbau diese DLL bei geschlossenem Spiel zurückkopieren; Spielstände mit Cherrypeater erst wieder mit 0.1.28 oder einer kompatiblen neueren Version laden. LawnGlove bleibt unverändert. `fusionen.txt` enthält nun 646 offene Rezepte für 535 Ergebnisse; das vollständige Quellenarchiv bleibt unverändert.


## Änderung 0.1.29: Split Cherry und Gatling Cherry

Alle drei Quellenrezepte für Split Cherry (1108) und alle vier für Gatling Cherry (1017) sind implementiert. Die aktiven Rezepte stehen oben; beide Zutatenreihenfolgen gelten jeweils. Zuerst wurde Split Cherry mit bereits verfügbaren Zutaten ergänzt; anschließend ist auch Peashooter + Split Cherry für Gatling Cherry verfügbar. Es bleiben keine gesperrten Alternativrezepte dieser beiden Ergebnisse offen. Insgesamt sind 29 Rezepte aktiv; die offene Liste enthält 639 Rezepte für 533 Ergebnisse. Das vollständige Quellenarchiv bleibt unverändert.

Split Cherry verschießt rosa Kirschkugeln: eine nach vorn, zwei gestaffelt nach hinten, je 40 Direktschaden. Rückwärtige Kugeln prallen am linken Spielfeldrand nach rechts zurück. Die bestehende gekoppelte Split-Pea-Zielsuche lässt beide Köpfe schießen, sobald eine Seite ein Ziel besitzt. Gatling Cherry verschießt vier gestaffelte Kirschkugeln mit je 40 Direktschaden. Der lokale Almanach nennt jeweils 1,5 Sekunden; der Port erhält native Split-Pea-/Gatling-Pea-Taktung samt zufälliger Verkürzung um 0–14 Simulationsticks. Beide verwenden den gemeinsamen Kirsch-Trefferpfad ohne Flächenexplosion oder Torchwood-Konvertierung. Es gibt keine zusätzliche Schusserzeugung durch Originalanimationen.

Originaldarstellung aus dem lokalen 4.0.5.2-Bundle:

- Split Cherry: CherrySplitPrefab 37192, Transform 120510, Script globalgamemanagers.assets:3373 CherrySplit, Animator 6362, Clips 4333/4334; 26 Knoten, 22 Sprites.
- Gatling Cherry: CherryGatlingPrefab 63745, Transform 92857, Script globalgamemanagers.assets:3282 CherryGatling, Animator 6591, Clips 4902/4903/4904; 22 Knoten, 18 Sprites.

Beide Modelle enthalten Originalhierarchie, Pivots, Sprite-Reihenfolge und additive Animationskurven. Leerlaufgeschwindigkeit ist 1,3. Der gemeinsame Schattenbezug beträgt wieder 0,88 × 7 × 0,23 = 1,4168 Quelleinheiten; die vorhandene Originalkirschkugel 61266 wird wiederverwendet. Gatling Cherrys zusätzlicher Clip `shooting` ist vollständig exportiert und eingebettet. Dieser Clip gehört zum gesonderten Schnellfeuerzustand, der normale Viererschuss verwendet `shoot`. Quellen-Modusupgrades und die bislang nicht portierten 15-Sekunden-Klickupgrades von Cherrypeater und Split Cherry bleiben außerhalb dieser Rezeptportierung. Beide Ergebnisse sind über sämtliche Quellenrezepte verfügbar; Gatling Cherrybomber bleibt offen.

Die Speicherkennungen sind Splitpea + Cherrybomb beziehungsweise Gatlingpea + Cherrybomb. Native Pflanzenfelder speichern diese Paare; beim Laden baut die Mod Darstellung und Kirschverhalten neu auf. Rezeptabfragen verwenden immer FusionPlant.From und lösen keine gewöhnlichen Split-Pea-/Gatling-Pea-Rezepte auf Varianten aus. Native Terrain-, Karten-, Handschuh-, Hervorhebungs- und Aufräumwege bleiben zuständig. Bei Split-Darstellungen berücksichtigt die gemeinsame Animation jetzt anhand des nativen Typs auch die zweite Kopfspur.

Speichergrenze: Bereits fliegende Kirschkugeln verlieren beim Laden ihre flüchtige Kirschzuordnung; rückwärtige Kugeln verlieren zusätzlich ihre Rückprallzuordnung. Neue Schüsse funktionieren wieder über den Variantenmarker. Spielstände mit diesen Varianten benötigen 0.1.29 oder eine kompatible neuere Mod-Version.

Quellenbelege, alle Rezeptregistrierungen mit RVAs, Almanachtexte, tatsächliche Methodenauszüge, Animatorcontroller, Asset-Prüfsummen und Offlinevorschauen liegen unter `references/split-cherry/` und `references/gatling-cherry/`. Das Recherchetool unterstützt nun `--type Namespace::Type`; `--type ::CherryGatling` wählt ausdrücklich den globalen Typ statt der gleichnamigen Klasse in GameLevel.RogueShooting. Unqualifizierte Mehrdeutigkeiten brechen weiterhin ab.

Release-Build: 0 Warnungen, 0 Fehler. Offline-Animationsblätter visuell geprüft; eingebettete Modelle und Sprites mit Exportdateien verglichen. Installierte DLL und Build sind binärgleich; Version 0.1.29 ist statisch geprüft. Vorversion/Rückbau: `backup/FusionMod-0.1.28.dll` bei geschlossenem Spiel nach `Mods/FusionMod.dll` kopieren. LawnGlove bleibt unverändert. Keine Spielstarts, Spieltests oder Änderungen an Spielständen und Profilen.

Manuelle Prüfung durch den Benutzer: alle sieben Rezepte, beide möglichen Kartenreihenfolgen und beide Handschuhrichtungen; bei zwei Mod-Zutaten den Handschuh verwenden. Cherry Bomb vor ihrem nativen Zünder fusionieren. Kartenkosten/Cooldown, Abbruch, ungültige Ziele, sichtbare Hervorhebung, Seerose/Topf/Kürbis, Abgrenzung zu normalen und Eisvarianten prüfen. Split Cherry mit Zielen nur links, nur rechts und beidseitig prüfen: drei Kirschkugeln, je 40 Schaden, Rückprall und Schaden nach Rückprall. Gatling Cherry auf vier Kugeln mit je 40 Schaden prüfen. Torchwood darf beide Kugelarten nicht verändern. Größe/Bodenlage, beide Kopfanimationen, Mündungsposition, Handschuhvorschau, Pause/Spielgeschwindigkeit, Tod und Pool-Wiederverwendung sowie Speichern/Laden prüfen. Keine Klickupgrades erwarten.

## Neue Fusionen in 21.2.0

Belch-shroom, Hungry-shroom, Trigger-shroom und Spicicle verwenden die Originalgrafiken und Animationen von Fusion 4.0.5.2. Verhalten, Projektile, Dünger und Quellenabweichungen sind im [Portnachweis](references/fume-guard-21-2-0/port-notes.md) beschrieben.
