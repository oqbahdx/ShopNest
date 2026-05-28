# Dev Scripts

## Start API Safely (fixed port `5149`)

From the repo root:

```bash
./scripts/dev-api.sh
```

This command:
- Keeps `http://localhost:5149` as the canonical local API URL.
- Stops only existing `ShopNest.API` listeners already bound to `5149`.
- Refuses to kill unrelated processes on `5149`.

## Manual Fallback

Inspect the current listener on port `5149`:

```bash
lsof -nP -iTCP:5149 -sTCP:LISTEN
```

If the listener is a stale `ShopNest.API` process, stop it:

```bash
kill -TERM <pid>
```

If it does not exit:

```bash
kill -KILL <pid>
```
