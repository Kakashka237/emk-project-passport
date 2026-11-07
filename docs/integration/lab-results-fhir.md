# EMK-305: прием результатов анализов в формате HL7 FHIR

Соответствие полей ресурса Observation модели результата анализа (ADR-001).

| FHIR Observation | Поле ЭМК | Правило |
|---|---|---|
| code.coding[system=loinc].code | TestCode | код LOINC обязателен |
| valueQuantity.value | Value | число, не пустое |
| valueQuantity.unit (UCUM) | Unit | единица из справочника UCUM |
| referenceRange.low / high | RefLow / RefHigh | при отсутствии — показатель без нормы |
| subject.reference | CardNumber | только номер карты, без ФИО |
| issued | IssuedAt | ISO 8601 с часовым поясом |
