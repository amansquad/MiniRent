"use client"

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import DeleteUserModal from "@/components/DeleteUserModal";

export default function UsersPage() {
    const [users, setUsers] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [isAdmin, setIsAdmin] = useState(false);
    const [creating, setCreating] = useState(false);
    const [editingUser, setEditingUser] = useState<any>(null);
    const [newUser, setNewUser] = useState({ fullName: "", username: "", password: "", email: "", phone: "", role: "Agent" });
    const [editFormData, setEditFormData] = useState({ fullName: "", email: "", phone: "", role: "", isActive: true });
    const [userToDelete, setUserToDelete] = useState<any>(null);

    useEffect(() => {
        if (typeof window === "undefined") return;

        const storedUser = localStorage.getItem("user");
        if (storedUser) {
            try {
                const parsed = JSON.parse(storedUser);
                if (parsed?.role === "Admin") {
                    setIsAdmin(true);
                } else {
                    // Redirect non-admins away from users page
                    window.location.href = "/dashboard";
                    return;
                }
            } catch {
                // ignore parse errors
            }
        } else {
            // Not logged in, redirect to auth
            window.location.href = "/auth";
            return;
        }

        const token = localStorage.getItem("token");
        fetch("/api/users", {
            headers: token ? { "Authorization": `Bearer ${token}` } : {}
        })
            .then(async (res) => {
                if (res.status === 401) {
                    window.location.href = "/auth?reason=login-required";
                    return;
                }
                if (!res.ok) throw new Error("Failed to fetch users");
                const json = await res.json();
                const data = Array.isArray(json) ? json : json.data;
                if (!Array.isArray(data)) {
                    throw new Error("Unexpected users response format");
                }
                setUsers(data);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    }, []);

    const handleCreateUser = async (e: React.FormEvent) => {
        e.preventDefault();
        if (typeof window === "undefined") return;

        try {
            setCreating(true);
            setError("");
            const token = localStorage.getItem("token");
            const res = await fetch("/api/auth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify({
                    username: newUser.username,
                    password: newUser.password,
                    fullName: newUser.fullName,
                    email: newUser.email,
                    phone: newUser.phone,
                    role: newUser.role,
                }),
            });

            const json = await res.json().catch(() => null);
            if (!res.ok) {
                throw new Error(json?.error || json?.message || "Failed to create user");
            }

            setNewUser({ fullName: "", username: "", password: "", email: "", phone: "", role: "Agent" });
            // Refresh the list
            const token2 = localStorage.getItem("token");
            const refreshRes = await fetch("/api/users", {
                headers: token2 ? { "Authorization": `Bearer ${token2}` } : {}
            });
            const refreshData = await refreshRes.json();
            const refreshUsers = Array.isArray(refreshData) ? refreshData : refreshData.data;
            setUsers(refreshUsers || []);
        } catch (err: any) {
            setError(err.message ?? String(err));
        } finally {
            setCreating(false);
        }
    };

    const handleEditUser = (user: any) => {
        setEditingUser(user);
        setEditFormData({
            fullName: user.fullName || "",
            email: user.email || "",
            phone: user.phone || "",
            role: user.role || "Agent",
            isActive: user.isActive !== undefined ? user.isActive : true,
        });
    };

    const handleSaveEdit = async () => {
        if (!editingUser || typeof window === "undefined") return;

        try {
            setError("");
            const token = localStorage.getItem("token");
            const res = await fetch(`/api/users/${editingUser.id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify(editFormData),
            });

            const json = await res.json().catch(() => null);
            if (!res.ok) {
                throw new Error(json?.error || json?.message || "Failed to update user");
            }

            setEditingUser(null);
            // Refresh the list
            const token2 = localStorage.getItem("token");
            const refreshRes = await fetch("/api/users", {
                headers: token2 ? { "Authorization": `Bearer ${token2}` } : {}
            });
            const refreshData = await refreshRes.json();
            const refreshUsers = Array.isArray(refreshData) ? refreshData : refreshData.data;
            setUsers(refreshUsers || []);
        } catch (err: any) {
            setError(err.message ?? String(err));
        }
    };

    const handleDeleteUser = async () => {
        if (!userToDelete || typeof window === "undefined") return;

        try {
            setError("");
            const token = localStorage.getItem("token");
            const res = await fetch(`/api/users/${userToDelete.id}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
            });

            if (!res.ok) {
                const json = await res.json().catch(() => null);
                throw new Error(json?.error || json?.message || "Failed to delete user");
            }

            // Refresh the list
            const token2 = localStorage.getItem("token");
            const refreshRes = await fetch("/api/users", {
                headers: token2 ? { "Authorization": `Bearer ${token2}` } : {}
            });
            const refreshData = await refreshRes.json();
            const refreshUsers = Array.isArray(refreshData) ? refreshData : refreshData.data;
            setUsers(refreshUsers || []);
            setUserToDelete(null);
        } catch (err: any) {
            setError(err.message ?? String(err));
        }
    };

    return (
        <div className="p-8 space-y-6">
            <h1 className="text-2xl font-bold">Users</h1>

            {error && <div className="text-red-500 text-sm">{error}</div>}

            {editingUser && (
                <div className="border rounded-lg p-4 bg-card">
                    <h2 className="font-semibold mb-4">Edit User: {editingUser.username}</h2>
                    <div className="space-y-3">
                        <div>
                            <label className="block text-sm font-medium mb-1">Full Name</label>
                            <input
                                type="text"
                                className="w-full border rounded px-2 py-1"
                                value={editFormData.fullName}
                                onChange={(e) => setEditFormData({ ...editFormData, fullName: e.target.value })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Email</label>
                            <input
                                type="email"
                                className="w-full border rounded px-2 py-1"
                                value={editFormData.email}
                                onChange={(e) => setEditFormData({ ...editFormData, email: e.target.value })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Phone</label>
                            <input
                                type="tel"
                                className="w-full border rounded px-2 py-1"
                                value={editFormData.phone}
                                onChange={(e) => {
                                    const val = e.target.value.replace(/\D/g, '').slice(0, 10);
                                    setEditFormData({ ...editFormData, phone: val });
                                }}
                                maxLength={10}
                                placeholder="10 digits number"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Role</label>
                            <select
                                className="w-full border rounded px-2 py-1"
                                value={editFormData.role}
                                onChange={(e) => setEditFormData({ ...editFormData, role: e.target.value })}
                            >
                                <option value="Agent">Agent</option>
                                <option value="Admin">Admin</option>
                            </select>
                        </div>
                        <div>
                            <label className="flex items-center gap-2">
                                <input
                                    type="checkbox"
                                    checked={editFormData.isActive}
                                    onChange={(e) => setEditFormData({ ...editFormData, isActive: e.target.checked })}
                                />
                                Active
                            </label>
                        </div>
                        <div className="flex gap-2">
                            <button
                                onClick={handleSaveEdit}
                                className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
                            >
                                Save
                            </button>
                            <button
                                onClick={() => setEditingUser(null)}
                                className="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400"
                            >
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {isAdmin && (
                <form onSubmit={handleCreateUser} className="space-y-2 border rounded-lg p-4 max-w-xl bg-card">
                    <h2 className="font-semibold">Create new user</h2>
                    <div className="flex flex-col gap-2">
                        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:flex-wrap">
                            <input
                                className="border rounded px-2 py-1 flex-1 min-w-[150px]"
                                placeholder="Full name"
                                value={newUser.fullName}
                                onChange={(e) => setNewUser({ ...newUser, fullName: e.target.value })}
                                required
                            />
                            <input
                                className="border rounded px-2 py-1 flex-1 min-w-[150px]"
                                placeholder="Username"
                                value={newUser.username}
                                onChange={(e) => setNewUser({ ...newUser, username: e.target.value })}
                                required
                            />
                            <input
                                type="password"
                                className="border rounded px-2 py-1 flex-1 min-w-[150px]"
                                placeholder="Password"
                                value={newUser.password}
                                onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
                                required
                            />
                        </div>
                        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:flex-wrap">
                            <input
                                type="email"
                                className="border rounded px-2 py-1 flex-1 min-w-[150px]"
                                placeholder="Email"
                                value={newUser.email}
                                onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
                                required
                            />
                            <input
                                type="tel"
                                className="border rounded px-2 py-1 flex-1 min-w-[150px]"
                                placeholder="Phone"
                                value={newUser.phone}
                                onChange={(e) => {
                                    const val = e.target.value.replace(/\D/g, '').slice(0, 10);
                                    setNewUser({ ...newUser, phone: val });
                                }}
                                maxLength={10}
                                required
                            />
                            <select
                                className="border rounded px-2 py-1 flex-1 min-w-[150px]"
                                value={newUser.role}
                                onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
                                required
                            >
                                <option value="Agent">Agent</option>
                                <option value="Admin">Admin</option>
                            </select>
                        </div>
                        <button
                            type="submit"
                            className="bg-blue-600 text-white px-3 py-1 rounded"
                            disabled={creating}
                        >
                            {creating ? "Creating..." : "Create user"}
                        </button>
                    </div>
                    <p className="text-xs text-muted-foreground">
                        Select role: Agent for basic user permissions, Admin for full access.
                    </p>
                </form>
            )}

            {loading ? (
                <div>Loading...</div>
            ) : error ? (
                <div className="text-red-500">{error}</div>
            ) : (
                Array.isArray(users) && users.length === 0 ? (
                    <div className="text-muted-foreground">No users yet. Invite or create users to see them here.</div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {Array.isArray(users) && users.map((user) => (
                            <Card key={user.id} className="p-4">
                                <div className="font-semibold">{user.fullName}</div>
                                <div className="text-muted-foreground text-sm">{user.email || "No email"}</div>
                                <div className="text-muted-foreground text-sm">{user.phone || "No phone"}</div>
                                <div className="text-muted-foreground text-xs mt-2">
                                    Role: {user.role} | {user.isActive ? "Active" : "Inactive"}
                                </div>
                                {isAdmin && (
                                    <div className="flex gap-2 mt-3">
                                        <button
                                            onClick={() => handleEditUser(user)}
                                            className="px-3 py-1 bg-blue-600 text-white text-sm rounded hover:bg-blue-700"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => setUserToDelete(user)}
                                            className="px-3 py-1 bg-red-600 text-white text-sm rounded hover:bg-red-700"
                                        >
                                            Delete
                                        </button>
                                    </div>
                                )}
                            </Card>
                        ))}
                    </div>
                )
            )}

            <DeleteUserModal
                isOpen={!!userToDelete}
                onClose={() => setUserToDelete(null)}
                onConfirm={handleDeleteUser}
                userName={userToDelete?.fullName || ""}
                userEmail={userToDelete?.email || "No email"}
            />
        </div>
    );
}
