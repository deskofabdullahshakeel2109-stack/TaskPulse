import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import "./Dashboard.css";

function Dashboard() {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const role = user?.role || "Member";

    const isAdmin = role.toLowerCase() === "admin";
    const isManager = role.toLowerCase() === "manager";

    return (
        <div className="dashboard-page">

            {/* Sidebar */}
            <aside className="dashboard-sidebar">

                <div className="sidebar-brand">
                    <h1>
                        Task<span>Pulse</span>
                    </h1>
                </div>

                <nav className="sidebar-nav">

                    <button
                        className="nav-item active"
                        onClick={() => navigate("/dashboard")}
                    >
                        <span className="nav-icon">⌂</span>
                        Dashboard
                    </button>

                    <button
                        className="nav-item"
                        onClick={() => navigate("/projects")}
                    >
                        <span className="nav-icon">▣</span>
                        Projects
                    </button>

                    <button
                        className="nav-item"
                        onClick={() => navigate("/tasks")}
                    >
                        <span className="nav-icon">✓</span>
                        Tasks
                    </button>

                    <button
                        className="nav-item"
                        onClick={() => navigate("/notifications")}
                    >
                        <span className="nav-icon">●</span>
                        Notifications
                    </button>

                    {(isAdmin || isManager) && (
                        <button
                            className="nav-item"
                            onClick={() => navigate("/reports")}
                        >
                            <span className="nav-icon">▤</span>
                            Reports
                        </button>
                    )}

                    {isAdmin && (
                        <button
                            className="nav-item"
                            onClick={() => navigate("/users")}
                        >
                            <span className="nav-icon">♙</span>
                            Users
                        </button>
                    )}

                    {isAdmin && (
                        <button
                            className="nav-item"
                            onClick={() => navigate("/activity-logs")}
                        >
                            <span className="nav-icon">☷</span>
                            Activity Logs
                        </button>
                    )}

                </nav>

                <div className="sidebar-bottom">

                    <button
                        className="nav-item"
                        onClick={handleLogout}
                    >
                        <span className="nav-icon">↪</span>
                        Logout
                    </button>

                </div>

            </aside>

            {/* Main Content */}
            <main className="dashboard-main">

                {/* Top Header */}
                <header className="dashboard-header">

                    <div>
                        <h2>Dashboard</h2>
                        <p>
                            Welcome back, {user?.fullName || user?.email}
                        </p>
                    </div>

                    <div className="user-info">

                        <div className="user-avatar">
                            {(user?.fullName || user?.email || "U")
                                .charAt(0)
                                .toUpperCase()}
                        </div>

                        <div className="user-details">
                            <strong>
                                {user?.fullName || "User"}
                            </strong>

                            <span>
                                {role}
                            </span>
                        </div>

                    </div>

                </header>

                {/* Welcome Card */}
                <section className="welcome-card">

                    <div>
                        <h3>
                            Good to see you again!
                        </h3>

                        <p>
                            Manage your projects, tasks and team
                            activities from one place.
                        </p>
                    </div>

                    <div className="welcome-role">
                        {role}
                    </div>

                </section>

                {/* Statistics */}
                <section className="dashboard-stats">

                    <div className="stat-card">
                        <div className="stat-icon">▣</div>
                        <div>
                            <span>Projects</span>
                            <strong>0</strong>
                        </div>
                    </div>

                    <div className="stat-card">
                        <div className="stat-icon">✓</div>
                        <div>
                            <span>Total Tasks</span>
                            <strong>0</strong>
                        </div>
                    </div>

                    <div className="stat-card">
                        <div className="stat-icon">◷</div>
                        <div>
                            <span>Pending Tasks</span>
                            <strong>0</strong>
                        </div>
                    </div>

                    <div className="stat-card">
                        <div className="stat-icon">✓</div>
                        <div>
                            <span>Completed</span>
                            <strong>0</strong>
                        </div>
                    </div>

                </section>

                {/* Dashboard Content */}
                <section className="dashboard-grid">

                    <div className="dashboard-panel">

                        <div className="panel-header">
                            <div>
                                <h3>Recent Projects</h3>
                                <p>Your latest project activity</p>
                            </div>

                            <button
                                onClick={() => navigate("/projects")}
                            >
                                View All
                            </button>
                        </div>

                        <div className="empty-state">
                            <div className="empty-icon">▣</div>

                            <h4>No projects yet</h4>

                            <p>
                                Your projects will appear here.
                            </p>

                            {(isAdmin || isManager) && (
                                <button
                                    onClick={() => navigate("/projects")}
                                    className="primary-small-button"
                                >
                                    Create Project
                                </button>
                            )}
                        </div>

                    </div>

                    <div className="dashboard-panel">

                        <div className="panel-header">
                            <div>
                                <h3>Recent Tasks</h3>
                                <p>Your latest task activity</p>
                            </div>

                            <button
                                onClick={() => navigate("/tasks")}
                            >
                                View All
                            </button>
                        </div>

                        <div className="empty-state">
                            <div className="empty-icon">✓</div>

                            <h4>No tasks yet</h4>

                            <p>
                                Your tasks will appear here.
                            </p>

                            {(isAdmin || isManager) && (
                                <button
                                    onClick={() => navigate("/tasks")}
                                    className="primary-small-button"
                                >
                                    View Tasks
                                </button>
                            )}
                        </div>

                    </div>

                </section>

            </main>

        </div>
    );
}

export default Dashboard;