# M450_GS

Modul 450 – Applikationen testen. Projekt von Gabriel Sarkis.

## Aufbau

```text
M450_GS.sln
src/
  M450_GS.App/          Applikationscode
tests/
  M450_GS.App.Tests/    Unit-Tests
```

## Voraussetzungen

- .NET 10 SDK

```bash
dotnet --version
```

## Bauen und starten

```bash
dotnet restore
dotnet build
dotnet run --project src/M450_GS.App
```

## Tests ausführen

```bash
dotnet test
```

Nützlich beim Arbeiten:

```bash
dotnet test --list-tests
dotnet test --verbosity normal
```

## Konventionen

- Tests folgen dem AAA-Muster: Arrange, Act, Assert.
- Testnamen nach dem Schema `Methode_Situation_ErwartetesErgebnis`,
  zum Beispiel `CalculateTotal_WithValidValues_ReturnsExpectedTotal`.
- Jeder Test ist unabhängig von den anderen und testet das öffentliche
  Verhalten, nicht die Implementierung.
