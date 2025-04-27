 # Simple Todo Management System

 ## Purpose
 The purpose of this project is to build a lightweight API that allows users to manage their personal tasks (todos). The system will enable basic task tracking, including creating new todos, viewing a list of todos, marking tasks as completed, and deleting tasks.

 ## Functional Requirements
 1. **Create Todo**
    - The system shall allow users to create a new todo item.
    - **Required fields**:
      - `id` (int): Unique identifier (auto-generated).
      - `name` (string): The task description or title.
      - `isDone` (bool): Whether the task is completed (default is `false`).

 2. **Retrieve Todos**
    - The system shall allow users to:
      - Retrieve a list of all todos.
      - Retrieve a specific todo by `id`.

 3. **Update Todo**
    - The system shall allow users to update:
      - The `name` of a todo.
      - The `isDone` status (mark as done or not done).

 4. **Delete Todo**
    - The system shall allow users to delete a todo by `id`.

 ## Non-Functional Requirements
 - The API shall return responses in JSON format.
 - The system shall be built using ASP.NET Core 9 Web API.
 - The application shall use in-memory data storage for simplicity (or SQLite optionally).
 - The API should follow RESTful standards.
 - Code must be readable and beginner-friendly for educational purposes.

 ## Example Todo JSON
 { "id": 1, "name": "Buy groceries", "isDone": false }