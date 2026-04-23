import TaskItem from './TaskItem.jsx';

function TaskList({ tasks, onCompleteTask, onReopenTask }) {
  if (tasks.length === 0) {
    return <p className="mt-6">No tasks yet.</p>;
  }

  return (
    <section className="mt-6">
      <h2 className="mb-3 text-xl font-semibold">Tasks</h2>
      <ul className="space-y-3">
        {tasks.map((task) => (
          <TaskItem
            key={task.id}
            task={task}
            onCompleteTask={onCompleteTask}
            onReopenTask={onReopenTask}
          />
        ))}
      </ul>
    </section>
  );
}

export default TaskList;
