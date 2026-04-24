const API_BASE_URL = '/api';

async function sendRequest(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, options);

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`);
  }

  return response.json();
}

export function getTasks() {
  return sendRequest('/tasks');
}

export function createTask(task) {
  return sendRequest('/tasks', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(task),
  });
}

export function completeTask(id) {
  return sendRequest(`/tasks/${id}/complete`, {
    method: 'POST',
  });
}

export function reopenTask(id) {
  return sendRequest(`/tasks/${id}/reopen`, {
    method: 'POST',
  });
}

export async function deleteTask(id) {
  const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
    method: 'DELETE',
  });

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`);
  }
}
