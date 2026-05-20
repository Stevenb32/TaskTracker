import { expect, APIRequestContext } from "@playwright/test";

export type CreateTaskRequest = {
  title: string;
  notes?: string;
};

export type TaskResponse = {
  id: string;
  title: string;
  notes: string | null;
  status: string;
  createdAt: string;
  completedAt: string | null;
  updatedAt: string | null;
};

export async function createTaskViaApi(request: APIRequestContext, task: CreateTaskRequest): Promise<TaskResponse> {
  const response = await request.post("http://localhost:5127/tasks", {
    data: task,
  });

  expect(response.status()).toBe(201);

  return await response.json();
}

export async function completeTaskViaApi(request: APIRequestContext, taskId: string): Promise<TaskResponse> {
  const response = await request.post(`http://localhost:5127/tasks/${taskId}/complete`);

  expect(response.status()).toBe(200);

  return await response.json();
}

export async function reopenTaskViaApi(request: APIRequestContext, taskId: string): Promise<TaskResponse> {
  const response = await request.post(`http://localhost:5127/tasks/${taskId}/reopen`);

  expect(response.status()).toBe(200);

  return await response.json();
}

export async function deleteTaskViaApi(request: APIRequestContext, taskId: string): Promise<void> {
  const response = await request.delete(`http://localhost:5127/tasks/${taskId}`);

  expect(response.status()).toBe(204);
}
