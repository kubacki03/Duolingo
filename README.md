## Duolingo-like – Aplikacja do nauki języków obcych

Duolingo-like to aplikacja webowa stworzona do nauki języków obcych, która wykorzystuje OpenAI API do generowania dynamicznych materiałów edukacyjnych, zadań praktycznych oraz quizów. Aplikacja oferuje możliwość nauki przez zabawę i interaktywność, dopasowując poziom trudności i rodzaj zadań do umiejętności użytkownika.

## Demo
https://youtu.be/dCM9Afysi50
---

## Funkcje

- Generowanie materiałów edukacyjnych** – Zajęcia, zadania i quizy są dynamicznie tworzone przez OpenAI API.
- Interaktywne quizy – Wykonuj testy sprawdzające Twoje umiejętności językowe.
- Śledzenie postępów – Zarejestruj swoje konto, aby monitorować postępy w nauce.
- Nauka słówek – Codzienne zadania i powtórki, dostosowane do Twojego poziomu.
- System logowania – Zabezpiecz swoje konto za pomocą ASP.NET Core Identity.
- Zarządzanie danymi – Przechowywanie wyników użytkowników i historii nauki z wykorzystaniem Entity Framework.

---

##  Technologie

- **ASP.NET Core** – Framework do budowy aplikacji webowej.
- **Entity Framework Core** – ORM do zarządzania bazą danych.
- **ASP.NET Core Identity** – System logowania i autoryzacji.
- **OpenAI API** – Generowanie dynamicznych materiałów edukacyjnych i quizów.
- **JavaScript** – Dla interaktywnych elementów front-endowych (np. quizy).
- **TailwindCSS** – Framework CSS do stylizacji aplikacji.

---

##  Instalacja

Aby uruchomić aplikację lokalnie:

1. Sklonuj repozytorium

```bash
git clone https://github.com/kubacki03/Duolingo.git
2. Przejdz do folderu projektu
cd Duolingo
3. Dodaj zmienne środowiskowe w tym klucze API
4. Skompiluj projekt
dotnet build
5. Uruchom projekt
dotnet run
