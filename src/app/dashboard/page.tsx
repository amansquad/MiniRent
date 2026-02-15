"use client";

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";

type DashboardStats = {
  totalProperties: number;
  activeRentals: number;
  newInquiries: number;
  monthlyRevenue: number;
  recentActivities: any[];
};

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (typeof window === "undefined") return;

    const token = window.localStorage.getItem("token");

    (async () => {
      try {
        const res = await fetch("/api", {
          headers: token ? { Authorization: `Bearer ${token}` } : {},
        });

        if (res.status === 401) {
          window.location.href = "/auth?reason=login-required";
          return;
        }

        if (!res.ok) throw new Error("Failed to fetch dashboard stats");

        const data: any = await res.json();
        setStats({
          totalProperties: data.totalProperties ?? 0,
          activeRentals: data.activeRentals ?? 0,
          newInquiries: data.newInquiries ?? 0,
          monthlyRevenue: data.monthlyRevenue ?? 0,
          recentActivities: data.recentActivities ?? [],
        });
        setLoading(false);
      } catch (err: any) {
        setError(err.message ?? String(err));
        setLoading(false);
      }
    })();
  }, []);

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold mb-8">Dashboard</h1>
      {loading ? (
        <div>Loading...</div>
      ) : error ? (
        <div className="text-red-500">{error}</div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
            <Card className="p-4">
              <div className="text-sm text-muted-foreground">Total Properties</div>
              <div className="text-3xl font-bold mt-2">{stats?.totalProperties}</div>
            </Card>
            <Card className="p-4">
              <div className="text-sm text-muted-foreground">Active Rentals</div>
              <div className="text-3xl font-bold mt-2">{stats?.activeRentals}</div>
            </Card>
            <Card className="p-4">
              <div className="text-sm text-muted-foreground">Open Inquiries</div>
              <div className="text-3xl font-bold mt-2">{stats?.newInquiries}</div>
            </Card>
            <Card className="p-4">
              <div className="text-sm text-muted-foreground">Monthly Revenue</div>
              <div className="text-3xl font-bold mt-2">${stats?.monthlyRevenue?.toLocaleString()}</div>
            </Card>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            <Card className="p-6">
              <h2 className="text-xl font-semibold mb-4">Recent Activity</h2>
              <div className="space-y-4">
                {stats?.recentActivities?.length === 0 ? (
                  <div className="text-muted-foreground">No recent activity.</div>
                ) : (
                  stats?.recentActivities?.map((activity, i) => (
                    <div key={i} className="flex justify-between items-start border-b pb-2 last:border-0">
                      <div>
                        <div className="font-medium">{activity.description}</div>
                        <div className="text-xs text-muted-foreground">by {activity.userName}</div>
                      </div>
                      <div className="text-xs text-muted-foreground">
                        {new Date(activity.timestamp).toLocaleDateString()}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </Card>
          </div>
        </>
      )}
    </div>
  );
}

