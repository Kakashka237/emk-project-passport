# АИС «Электронная медицинская карта» — паспорт артефактов и протоколов

Учебный проект МДК 02.02 (практическая работа № 8, вариант 2): модуль записи студентов к врачам поликлиники.
Мельник И. И., группа И14-23-9.

| Артефакт | Категория | Где лежит |
|---|---|---|
| C4 Containers Diagram | архитектурный | [docs/architecture/c4-container.puml](docs/architecture/c4-container.puml) |
| OpenAPI-спецификация шлюза | контрактный | [api/openapi.yaml](api/openapi.yaml) |
| Тесты `AppointmentServiceTests` | кодовой / тестовый | [tests/Emk.Core.Tests](tests/Emk.Core.Tests/AppointmentServiceTests.cs) |
| Дефект EMK-271 | управленческий | раздел Issues репозитория |
| ADR-001: HL7 FHIR R4 | решение | [docs/adr/ADR-001-hl7-fhir.md](docs/adr/ADR-001-hl7-fhir.md) |

Полный паспорт — [PASSPORT.md](PASSPORT.md). Правила работы с веткой и Pull Request — [CONTRIBUTING.md](CONTRIBUTING.md).

Проверки CI на каждый push и Pull Request: тесты бизнес-логики, валидация `api/openapi.yaml` (openapi-spec-validator) и статический анализ безопасности CodeQL. Локально:

```
dotnet test tests/Emk.Core.Tests
```
