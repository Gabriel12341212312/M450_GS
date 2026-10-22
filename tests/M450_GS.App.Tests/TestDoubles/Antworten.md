# Test Doubles – Antworten

## Aufgabe 1

Für `ISeatAvailability` gibt es noch keine Implementierung, also schreibe ich im Testprojekt einen Stub, der fixe Werte zurückgibt. Die Schnittstelle sagt nur, dass man einen Veranstaltungscode reingibt und eine Zahl zurückbekommt. Meine Annahme: Die Zahl sind die Plätze, die man im Moment noch reservieren kann, und sie ist nie negativ.

## Aufgabe 2

Meine Doubles merken sich ihre Aufrufe. Nach der Exception prüfe ich, dass diese Listen leer sind. So sehe ich, dass nichts aufgerufen wurde, ohne den Produktionscode zu ändern.

## Aufgabe 3

Der Stub gibt dem Service Daten, der Spy schreibt mit, was der Service aufgerufen hat.

## Aufgabe 5

Für die Verfügbarkeit nehme ich meinen Stub und für die Bestätigung meinen Spy, weil ich die schon habe und leere Listen einfach zu prüfen sind. Für das Repository nehme ich Moq, weil ich mit `Times.Never()` direkt prüfen kann, dass nichts gespeichert wurde.

## Gruppenreservierung

Wenn `Send` nach dem Speichern eine Exception wirft, ist die Reservierung gespeichert, aber der Kunde hat keine Bestätigung. Wenn er nochmals reserviert, gibt es sie doppelt. Lösung: Die Reservierung als "Bestätigung offen" markieren und die Bestätigung später nochmals schicken.
