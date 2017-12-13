# FreeNames

Утилита для поиска свободных имен компьютеров и IP-адресов.

### Критерии поиска

Имя считается свободным, если в DNS нет такой записи и в Active Directory нет такого компьютера.

Адрес считается свободным, если в DNS нет такой записи и этот адрес не отвечает на пинги.

### Параметры поиска

В шаблоне имени должен присутствовать счетчик "[C]".

Домен используется для поиска и в DNS, и в Active Directory.

При поиске адресов сканируются подсети с маской 24. Подсети необходимо указывать в формате "10.10.10" по одной на строку.

Параметры поиска по умолчанию хранятся в файле Settings.xml.

### Требования

- .Net 4.5
- Имена должны разрешаться на указанных в настройках системы DNS.
- Пользователь, запустивший утилиту, должен иметь права на чтение в указанном домене Active Directory.