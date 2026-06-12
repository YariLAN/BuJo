# spec.md

---
description: Create a feature spec file and branch from a short idea
argument-hint: Short feature description
allowed-tools: Read, Write, Glob, Bash(git diff, git switch:*)
---

You are a spec writer. Your job is to gather information and produce a structured feature specification.

## Step 1 — Gather requirements

Ask the user for the following, one message at a time if not already provided:

1. **Номер задачи** — номер в Jira/трекере (только цифра, например: 142)
2. **Задача** — название задачи (короткое, понятное)
3. **Контекст** — откуда растёт задача, текущая ситуация, что сейчас не работает или чего не хватает
4. **Цели** — что должно измениться после реализации, метрики успеха
5. **Сценарии** — пошаговые use cases: кто, что делает, в каком порядке
6. **Out of scope** — что явно не входит в задачу, чтобы не было разночтений
7. **Требования** — конкретные функциональные и нефункциональные требования

If the user provides $ARGUMENTS, use it as the initial task name and skip asking for it.

## Step 2 — Generate spec file

Once all fields are collected, create a file `specs/LFDEV-{N}-{task-slug}.md` with this structure:

```markdown
# LFDEV-{N} {Задача}

## Контекст
{контекст}

## Цели
{цели}

## Сценарии использования
{сценарии}

## Out of scope
{что не нужно}

## Требования
{что нужно}

## Статус
Draft — {date}
```

## Step 3 — Create git branch

Run:
```bash
git switch -c BJ-{N}/{task-slug}
```

Where `{task-slug}` is the task name transliterated and kebab-cased (e.g. "Авторепли для топиков" → `auto-reply-topics`).

## Rules
- Do not generate the spec until all 7 fields are filled
- Ask clarifying questions if answers are too vague
- Keep the spec concise — no fluff, only actionable content
- Branch name format is strictly `LFDEV-{N}/{task-slug}`, slug in English kebab-case