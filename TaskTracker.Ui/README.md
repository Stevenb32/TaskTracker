# TaskTracker.Ui

Basic React frontend for the Task Tracker API.

## Run

Install dependencies if needed:

```sh
npm install
```

Start the Vite dev server:

```sh
npm run dev
```

The dev server proxies `/api` requests to `http://localhost:5127`.

## API endpoints used

- `GET /tasks`
- `POST /tasks`
- `POST /tasks/{id}/complete`
- `POST /tasks/{id}/reopen`
- `DELETE /tasks/{id}`
