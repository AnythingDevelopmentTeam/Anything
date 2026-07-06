# Build & Dev Notes

## Сборка searchengine.dll (Rust → WPF)

MinGW-w64 нужен, т.к. MSVC linker не установлен.

```powershell
# 1. Установить MinGW (если нет)
winget install --id BrechtSanders.WinLibs.POSIX.UCRT --accept-package-agreements --exact

# 2. Скопировать в ASCII-путь (иначе ld падает на кириллице)
Copy-Item -Path "searchengine" -Destination "C:\Anything\searchengine" -Recurse -Force

# 3. Переключиться на GNU toolchain
rustup default stable-x86_64-pc-windows-gnu

# 4. Найти dlltool.exe и добавить в PATH
# WinLibs обычно ставится в C:\Program Files\WinLibs\...\mingw64\bin\
$mingw = Get-ChildItem -Path "C:\Program Files\WinLibs" -Filter "dlltool.exe" -Recurse | Select-Object -First 1 -ExpandProperty DirectoryName
$env:PATH = "$mingw;$env:PATH"

# 5. Собрать
Set-Location -LiteralPath "C:\Anything\searchengine"
cargo build --release

# 6. Скопировать результат
Copy-Item "C:\Anything\searchengine\target\release\searchengine.dll" "Anything-UI-WPF\searchengine.dll" -Force

# 7. Вернуть toolchain обратно
rustup default stable-x86_64-pc-windows-msvc
```

## Сборка и запуск WPF

```powershell
# .NET 10 SDK уже установлен
dotnet build "Anything-UI-WPF\Anything-UI-WPF.csproj"
dotnet run --project "Anything-UI-WPF\Anything-UI-WPF.csproj"
```

## Что сделано (сессия 06.07.2026)

### Quick wins
- Fix Rebuild Index button in WPF SettingsWindow (callback wiring)
- GTK theme persistence (~/.config/anything/theme)
- Remove dead ThemeIconConverter/ThemeTooltipConverter

### Medium tasks
- File metadata: size + last modified columns in results (lazy from FileInfo)
- Context menu: Open, Open Folder, Copy Path, Copy File, Properties
- Search history: last 20 queries, persisted in settings.json
- Tray icon: minimize on close, restore via tray

### Что ещё можно сделать
- Перевести трей-меню на русский (сейчас берётся из _lang, проверить)
- Иконки файлов в результатах (SHGetFileInfo/Icon.ExtractAssociatedIcon)
- Сортировка по колонкам (size/date)
- Глобальный хоткей Ctrl+Space
- Выбор папок для индексации
- Поиск по содержимому
