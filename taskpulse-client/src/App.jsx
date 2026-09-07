import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";

import Login from "./pages/Auth/Login";
import Dashboard from "./pages/Dashboard/Dashboard";
import Projects from "./pages/Projects/Projects";
import ProjectDetails from "./pages/Projects/ProjectDetails";
import Tasks from "./pages/Tasks/Tasks";
import TaskDetails from "./pages/Tasks/TaskDetails";
import CreateTask from "./pages/Tasks/CreateTask";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />

                <Route path="/dashboard" element={<Dashboard />} />

                <Route path="/projects" element={<Projects />} />

                <Route
                    path="/projects/:projectId"
                    element={<ProjectDetails />}
                />

                <Route
                    path="/tasks"
                    element={<Tasks />}
                />

                <Route
                    path="/projects/:projectId/tasks"
                    element={<Tasks />}
                />

                <Route
                    path="/projects/:projectId/tasks/:taskId"
                    element={<TaskDetails />}
                />

                <Route
                    path="/"
                    element={<Navigate to="/login" replace />}
                />
                <Route
                    path="/projects/:projectId/tasks/create"
                    element={<CreateTask />}
                />
                <Route
                    path="*"
                    element={<Navigate to="/login" replace />}
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App; 