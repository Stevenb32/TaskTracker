import { useState } from 'react';

function TaskForm({ onCreateTask }) {
  const [title, setTitle] = useState('');
  const [notes, setNotes] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  async function handleSubmit(event) {
    event.preventDefault();

    const trimmedTitle = title.trim();
    const trimmedNotes = notes.trim();

    if (!trimmedTitle) {
      return;
    }

    setIsSaving(true);

    try {
      await onCreateTask({
        title: trimmedTitle,
        notes: trimmedNotes || null,
      });

      setTitle('');
      setNotes('');
    } catch {
      // App shows the error message.
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4 border p-4">
      <div>
        <label htmlFor="title" className="mb-1 block font-medium">
          Title
        </label>
        <input
          id="title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          className="w-full border px-2 py-1"
          maxLength="100"
        />
      </div>

      <div>
        <label htmlFor="notes" className="mb-1 block font-medium">
          Notes
        </label>
        <textarea
          id="notes"
          value={notes}
          onChange={(event) => setNotes(event.target.value)}
          className="w-full border px-2 py-1"
          maxLength="500"
          rows="3"
        />
      </div>

      <button
        type="submit"
        disabled={isSaving}
        className="bg-blue-600 text-white px-3 py-1 border border-blue-700 hover:bg-blue-700 rounded"
      >
        {isSaving ? 'Saving...' : 'Create Task'}
      </button>
    </form>
  );
}

export default TaskForm;
