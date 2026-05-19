import { useState } from 'react';

const TITLE_MAX_LENGTH = 100;
const NOTES_MAX_LENGTH = 500;

function TaskForm({ onCreateTask }) {
  const [title, setTitle] = useState('');
  const [notes, setNotes] = useState('');
  const [showTitleRequired, setShowTitleRequired] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const titleRequiredMessage = showTitleRequired && !title.trim() ? 'Title is required' : '';
  const titleLimitMessage = title.length === TITLE_MAX_LENGTH ? `Title can only be ${TITLE_MAX_LENGTH} characters` : '';
  const notesLimitMessage = notes.length === NOTES_MAX_LENGTH ? `Notes can only be ${NOTES_MAX_LENGTH} characters` : '';
  const titleMessage = titleRequiredMessage || titleLimitMessage;
  const notesMessage = notesLimitMessage;
  const titleClassName = `w-full border px-2 py-1 ${titleMessage ? 'border-red-700' : ''}`;
  const notesClassName = `w-full border px-2 py-1 ${notesMessage ? 'border-red-700' : ''}`;

  async function handleSubmit(event) {
    event.preventDefault();

    const trimmedTitle = title.trim();
    const trimmedNotes = notes.trim();

    if (!trimmedTitle) {
      setShowTitleRequired(true);
      return;
    }

    if (title.length > TITLE_MAX_LENGTH) {
      return;
    }

    if (notes.length > NOTES_MAX_LENGTH) {
      return;
    }

    setShowTitleRequired(false);
    setIsSaving(true);

    try {
      await onCreateTask({
        title: trimmedTitle,
        notes: trimmedNotes || null,
      });

      setTitle('');
      setNotes('');
      setShowTitleRequired(false);
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
          className={titleClassName}
          maxLength={TITLE_MAX_LENGTH}
        />
        {titleMessage && <p className="mt-1 text-sm text-red-700">{titleMessage}</p>}
      </div>

      <div>
        <label htmlFor="notes" className="mb-1 block font-medium">
          Notes
        </label>
        <textarea
          id="notes"
          value={notes}
          onChange={(event) => setNotes(event.target.value)}
          className={notesClassName}
          maxLength={NOTES_MAX_LENGTH}
          rows="3"
        />
        {notesMessage && <p className="mt-1 text-sm text-red-700">{notesMessage}</p>}
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
