# Codex Task Handoff

Codex should create and fill this handoff when a fresh task is useful. The user should not need to reconstruct project context manually.

```text
Продолжаем Project Genesis из <полный путь к репозиторию>.

Ветка: <ветка>. Последний коммит: <хеш и название>. Текущее состояние git: <краткий status и известные чужие файлы>.

Прочитай README.md, 00_PROJECT_MANIFESTO.md, 02_GAME_VISION.md, 12_TECH_ARCHITECTURE.md, 14_CODEX_WORKING_RULES.md, 15_ROADMAP.md, 17_BACKLOG.md и 32_DEVELOPMENT_PLAN_RU.md, а также документ последнего завершённого спринта.

Не начинай писать код сразу.

Сначала:
1. проверь git status и не трогай известные посторонние файлы;
2. подтверди текущий этап и конкретную цель;
3. перечисли принятые решения, которые влияют на реализацию;
4. перечисли, что точно не входит в задачу;
5. предложи один маленький проверяемый шаг и выполняй его автономно.

Текущая цель: <одна цель>.

Уже сделано и важно не сломать: <краткий список>.

Вне задачи: <краткий список>.

Codex выполняет компиляцию, валидаторы и сложные технические проверки. Пользователю даются короткие шаги для обычной визуальной проверки в Unity. Не создавай финальный git-коммит без отдельного согласования пользователя.
```

The handoff should be concrete when used: replace every placeholder, include any current uncommitted ownership, and link the exact sprint document once it exists.
