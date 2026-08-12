import { useEffect, useState } from "react";
import { getTasks, createTask, updateTaskDetails, changeTaskPriority, completeTask, reopenTask, deleteTask } from "./api/tasksApi.js";
import TaskForm from "./components/TaskForm.jsx";
import TaskList from "./components/TaskList.jsx";

function App() {
  const [tasks, setTasks] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [prioritySavingTaskId, setPrioritySavingTaskId] = useState(null);

  async function loadTasks() {
    setIsLoading(true);
    setError("");

    try {
      const taskList = await getTasks();
      setTasks(taskList);
    } catch {
      setError("Could not load tasks. Make sure the API is running.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadTasks();
  }, []);

  async function handleCreateTask(task) {
    setError("");

    try {
      await createTask(task);
      await loadTasks();
    } catch {
      setError("Could not create the task.");
      throw new Error("Create task failed");
    }
  }

  async function handleCompleteTask(id) {
    setError("");

    try {
      await completeTask(id);
      await loadTasks();
    } catch {
      setError("Could not complete the task.");
    }
  }

  async function handleReopenTask(id) {
    setError("");

    try {
      await reopenTask(id);
      await loadTasks();
    } catch {
      setError("Could not reopen the task.");
    }
  }

  async function handleUpdateTask(id, task) {
    setError("");

    try {
      const updatedTask = await updateTaskDetails(id, task);

      setTasks((currentTasks) => currentTasks.map((currentTask) => (currentTask.id === updatedTask.id ? updatedTask : currentTask)));
    } catch {
      setError("Could not update the task.");
      throw new Error("Update task failed");
    }
  }

  async function handleChangePriority(id, priority) {
    setError("");
    setPrioritySavingTaskId(id);

    try {
      const updatedTask = await changeTaskPriority(id, priority);

      setTasks((currentTasks) => currentTasks.map((currentTask) => (currentTask.id === updatedTask.id ? updatedTask : currentTask)));
    } catch {
      setError("Could not update the task priority.");
    } finally {
      setPrioritySavingTaskId(null);
    }
  }

  async function handleDeleteTask(id) {
    setError("");

    try {
      await deleteTask(id);
      await loadTasks();
    } catch {
      setError("Could not delete the task.");
    }
  }

  return (
    <main className="mx-auto max-w-3xl p-6">
      <h1 className="mb-6 text-2xl font-bold">Task Tracker</h1>

      <TaskForm onCreateTask={handleCreateTask} />

      {isLoading && <p className="mt-4">Loading tasks...</p>}
      {error && <p className="mt-4 text-red-700">{error}</p>}

      {!isLoading && (
        <TaskList
          tasks={tasks}
          onCompleteTask={handleCompleteTask}
          onReopenTask={handleReopenTask}
          onUpdateTask={handleUpdateTask}
          onChangePriority={handleChangePriority}
          prioritySavingTaskId={prioritySavingTaskId}
          onDeleteTask={handleDeleteTask}
        />
      )}
    </main>
  );
}

export default App;
