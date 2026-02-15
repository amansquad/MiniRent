"use client";

import { useEffect, useState } from "react";
import { getUser } from "@/lib/auth";

export default function ProfilePage() {
  const [user, setUser] = useState<any>(null);
  const [editing, setEditing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phone: "",
  });

  useEffect(() => {
    const currentUser = getUser();
    if (!currentUser) {
      window.location.href = "/auth";
      return;
    }
    setUser(currentUser);
    setFormData({
      fullName: currentUser.fullName || "",
      email: currentUser.email || "",
      phone: currentUser.phone || "",
    });
  }, []);

  const handleSave = async () => {
    if (typeof window === "undefined") return;

    setLoading(true);
    setError("");

    try {
      const token = localStorage.getItem("token");
      const res = await fetch("/api/auth/profile", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...(token ? { "Authorization": `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(formData),
      });

      const data = await res.json().catch(() => null);
      if (!res.ok) {
        throw new Error(data?.message || data?.error || "Failed to update profile");
      }

      // Update local storage with new user data
      const updatedUser = { ...user, ...formData };
      localStorage.setItem("user", JSON.stringify(updatedUser));
      setUser(updatedUser);
      setEditing(false);
    } catch (err: any) {
      setError(err.message ?? String(err));
    } finally {
      setLoading(false);
    }
  };

  if (!user) {
    return <div className="p-8">Loading...</div>;
  }

  return (
    <div className="p-8 max-w-lg mx-auto">
      <h1 className="text-2xl font-bold mb-8">My Profile</h1>

      {error && <div className="mb-4 text-red-500 text-sm">{error}</div>}

      <div className="bg-white dark:bg-slate-900 rounded shadow p-6 space-y-4">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-lg font-semibold">Profile Information</h2>
          {!editing && (
            <button
              onClick={() => setEditing(true)}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Edit
            </button>
          )}
        </div>

        {editing ? (
          <>
            <div>
              <label className="block text-sm font-medium mb-1">Full Name</label>
              <input
                type="text"
                className="w-full rounded-md border px-3 py-2 text-sm"
                value={formData.fullName}
                onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Email</label>
              <input
                type="email"
                className="w-full rounded-md border px-3 py-2 text-sm"
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Phone</label>
              <input
                type="tel"
                className="w-full rounded-md border px-3 py-2 text-sm"
                value={formData.phone}
                onChange={(e) => {
                  const val = e.target.value.replace(/\D/g, '').slice(0, 10);
                  setFormData({ ...formData, phone: val });
                }}
                maxLength={10}
                placeholder="10 digits number"
                required
              />
            </div>
            <div className="flex gap-2 pt-2">
              <button
                onClick={handleSave}
                disabled={loading}
                className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 disabled:opacity-50"
              >
                {loading ? "Saving..." : "Save"}
              </button>
              <button
                onClick={() => {
                  setEditing(false);
                  setFormData({
                    fullName: user.fullName || "",
                    email: user.email || "",
                    phone: user.phone || "",
                  });
                  setError("");
                }}
                className="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400"
              >
                Cancel
              </button>
            </div>
          </>
        ) : (
          <>
            <div><b>Full Name:</b> {user.fullName}</div>
            <div><b>Username:</b> {user.username}</div>
            <div><b>Email:</b> {user.email || "Not set"}</div>
            <div><b>Phone:</b> {user.phone || "Not set"}</div>
            <div><b>Role:</b> {user.role}</div>
            <div><b>Active:</b> {user.isActive ? "Yes" : "No"}</div>
            <div><b>Created At:</b> {new Date(user.createdAt).toLocaleString()}</div>
          </>
        )}
      </div>
    </div>
  );
}
