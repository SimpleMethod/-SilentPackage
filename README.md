# SilentPackage
Projekt SilentPackage ma na celu umożliwić monitorowanie użytkownika systemu Windows 10.

Projekt wykonał Michał Młodawski (1ID21A)  [m.mlodawski@simplemethod.io](mailto:m.mlodawski@simplemethod.io)

Podział prac w zespole: Wszystko wykonał Michał Młodawski 

## 1. Lista funkcjonalności
1. Pobieranie listy procesów z nazwą, identyfikatorem procesu oraz czasem startu procesu.
2. Wykonywanie operacji wylogowania użytkownika z systemu bądź zamykania systemu lub jego ponowne uruchomienie.
3.  Wykonywanie zrzutów ekranu z możliwością wyboru ścieżki zapisu i jakości robionego zrzutu ekranu.
4.  Pobieranie listy plików ich czas utworzenia i modyfikacji o podanych rozszerzeniach w podanej lokalizacji wraz z podkatalogami.  
5. Przeglądanie historii przeglądania stron www z poziomu przeglądarki opartej o silnik Chrome (Nowa odsłona Edge, Google Chrome)
6. Możliwość blokowania uruchomienia określonych programów.
7. Ograniczenie czasu pracy użytkownika.
8.  Graficzny interfejs dostępny z poziomu przeglądarki pracujący w koncepcji „jednej strony” obsługujący zapytania AJAX.
9.  Dostęp do API w celu rozszerzenia projektu o dodatkowe punkty wyjścia telemetrii.
10. Architektura klient-serwer.

##  2. Spis technologii
-   .NET w aplikacji do zarządzania systemem Windows.
-   JAVA w kliencie umożliwiającym prezentację danych.
    -   Spring Framework

### 1. Użyte języki programowania
1.  Java wraz z framework Spring do warstwy serwerowej.
2.  C# w wersji .Net framework do warstwy kontroli.
3.  HTML z wykorzystaniem frameworka Bootstrap, JavaScript z frameworkiem jQuery oraz AngularJS.
4.  JSON jako format tekstowy dla przejrzystego uporządkowania danych.

### 2. Wykorzystane oprogramowanie przy projektowaniu i wdrażaniu projektu
1.  Adobe XD CC w celu szybkiego prototypowanie interfejsu graficznego.
2.  IntelliJ IDEA jako główne IDE do programowanie części serwerowej.
3.  JavaDoc do prowadzenia dokumentacji kodu.
4.  JetBrains WebStorm jako IDE do interfejsu graficznego.
5.  Visual Studio z rozszerzeniem ReSharper w celu stworzenia programu do pobierania telemetrii.

##  3. Opis warstwy serwerowej
Warstwa serwerowa wykonana by była w technologii Java z wykorzystaniem Framework Spring jej celem byłaby agregacja danych odebranych od klienta poprzez architekturę REST. Następnie dane byłby prezentowane w czasie rzeczywistym za pomocą interfejsu graficznego dostępnego z poziomu przeglądarki www.

 
##  4. Opis warstwy klienckiej
Warstwa kliencka zostałaby wykonana w technologii .NET Framework. Ma na celu zarządzaniem systemem Windows i jego monitoringiem. Agregowane dane poprzez zapytania HTTP byłby wysyłane do serwera. Plik konfiguracja odbywała się poprzez konfigurację pliku JSON albo poprzez zapytania HTTP wykorzystują architekturę REST.  Aplikację można ochronić przed usunięciem wykorzystując mechanizm TrustedInstaller albo poprzez ręczne odebranie użytkownikowi praw do modyfikacji pliku.
