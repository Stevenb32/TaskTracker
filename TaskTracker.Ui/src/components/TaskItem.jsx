function formatDate(value) {
  if (!value) {
    return '';
  }

  return new Date(value).toLocaleString();
}

function TaskItem({ task, onCompleteTask, onReopenTask, onDeleteTask }) {
  return (
    <li className="border p-4">
      <h3 className="font-semibold">{task.title}</h3>

      <p className="mt-2">{task.notes || 'No notes'}</p>

      <dl className="mt-3 space-y-1 text-sm">
        <div>
          <dt className="inline font-medium">Status: </dt>
          <dd className="inline">{task.status}</dd>
        </div>
        <div>
          <dt className="inline font-medium">Created: </dt>
          <dd className="inline">{formatDate(task.createdAt)}</dd>
        </div>
        {task.completedAt && (
          <div>
            <dt className="inline font-medium">Completed: </dt>
            <dd className="inline">{formatDate(task.completedAt)}</dd>
          </div>
        )}
      </dl>

      <div className="mt-3 flex gap-2">
        {task.status === 'Active' && (
          <button
            type="button"
            onClick={() => onCompleteTask(task.id)}
            className="border px-3 py-1"
          >
            Complete
          </button>
        )}

        {task.status === 'Completed' && (
          <button
            type="button"
            onClick={() => onReopenTask(task.id)}
            className="border px-3 py-1"
          >
            Reopen
          </button>
        )}

        <button
          type="button"
          onClick={() => onDeleteTask(task.id)}
          className="border px-3 py-1"
        >
          Delete
        </button>
      </div>
    </li>
  );
}

export default TaskItem;
