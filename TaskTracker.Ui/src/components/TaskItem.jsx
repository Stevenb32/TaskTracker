import { useState } from 'react';

function formatDate(value) {
  if (!value) {
    return '';
  }

  return new Date(value).toLocaleString();
}

function TaskItem({ task, onCompleteTask, onReopenTask, onUpdateTask, onDeleteTask }) {
  const [isEditing, setIsEditing] = useState(false);
  const [title, setTitle] = useState(task.title);
  const [notes, setNotes] = useState(task.notes || '');
  const [validationMessage, setValidationMessage] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  function startEditing() {
    setTitle(task.title);
    setNotes(task.notes || '');
    setValidationMessage('');
    setIsEditing(true);
  }

  function cancelEditing() {
    setTitle(task.title);
    setNotes(task.notes || '');
    setValidationMessage('');
    setIsEditing(false);
  }

  async function saveChanges(event) {
    event.preventDefault();

    const trimmedTitle = title.trim();
    const trimmedNotes = notes.trim();

    if (!trimmedTitle) {
      setValidationMessage('Title is required.');
      return;
    }

    setValidationMessage('');
    setIsSaving(true);

    try {
      await onUpdateTask(task.id, {
        title: trimmedTitle,
        notes: trimmedNotes || null,
      });

      setIsEditing(false);
    } catch {
      // App shows the error message.
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <li className="border p-4">
      {isEditing ? (
        <form onSubmit={saveChanges} className="space-y-3">
          <div>
            <label htmlFor={`edit-title-${task.id}`} className="mb-1 block font-medium">
              Title
            </label>
            <input
              id={`edit-title-${task.id}`}
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              className="w-full border px-2 py-1"
              maxLength="100"
            />
          </div>

          <div>
            <label htmlFor={`edit-notes-${task.id}`} className="mb-1 block font-medium">
              Notes
            </label>
            <textarea
              id={`edit-notes-${task.id}`}
              value={notes}
              onChange={(event) => setNotes(event.target.value)}
              className="w-full border px-2 py-1"
              maxLength="500"
              rows="3"
            />
          </div>

          {validationMessage && <p className="text-sm text-red-700">{validationMessage}</p>}

          <div className="flex gap-2">
            <button
              type="submit"
              disabled={isSaving}
              className="bg-blue-600 text-white px-3 py-1 border border-blue-700 hover:bg-blue-700 rounded"
            >
              {isSaving ? 'Saving...' : 'Save'}
            </button>

            <button
              type="button"
              onClick={cancelEditing}
              disabled={isSaving}
              className="bg-gray-200 text-black px-3 py-1 border border-gray-300 hover:bg-gray-300 rounded"
            >
              Cancel
            </button>
          </div>
        </form>
      ) : (
        <>
          <h3 className="font-semibold">{task.title}</h3>

          <p className="mt-2">{task.notes || 'No notes'}</p>
        </>
      )}

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

      {!isEditing && (
        <div className="mt-3 flex gap-2">
          {task.status === 'Active' && (
            <button
              type="button"
              onClick={() => onCompleteTask(task.id)}
              className="bg-green-500 text-white px-3 py-1 border border-green-600 hover:bg-green-600 rounded"
            >
              Complete
            </button>
          )}

          {task.status === 'Completed' && (
            <button
              type="button"
              onClick={() => onReopenTask(task.id)}
              className="bg-yellow-500 text-black px-3 py-1 border border-yellow-600 hover:bg-yellow-600 rounded"
            >
              Reopen
            </button>
          )}

          <button
            type="button"
            onClick={startEditing}
            className="bg-gray-200 text-black px-3 py-1 border border-gray-300 hover:bg-gray-300 rounded"
          >
            Edit
          </button>

          <button
            type="button"
            onClick={() => onDeleteTask(task.id)}
            className="bg-red-500 text-white px-3 py-1 border border-red-600 hover:bg-red-600 rounded"
          >
            Delete
          </button>
        </div>
      )}
    </li>
  );
}

export default TaskItem;
