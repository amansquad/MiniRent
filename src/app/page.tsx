"use client";

import { useEffect, useState } from "react";

declare const window: any;

export default function Home() {
  const [user, setUser] = useState<any | null>(null);

  useEffect(() => {
    if (typeof window === "undefined") return;
    const stored = window.localStorage.getItem("user");
    if (stored) {
      try {
        setUser(JSON.parse(stored));
      } catch {
        setUser(null);
      }
    }
  }, []);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-950 via-slate-900 to-slate-950 text-white">
      <div className="max-w-5xl mx-auto px-6 py-16 space-y-16">
        <section className="grid gap-10 md:grid-cols-2 items-center">
          <div className="space-y-4">
            <h1 className="text-3xl md:text-4xl font-bold leading-tight">
              Welcome to <span className="text-blue-400">MiniRent</span>
            </h1>
            <p className="text-sm md:text-base text-slate-300">
              MiniRent is a focused rental management dashboard for small and medium
              portfolios. Track your properties, active rentals, and tenant inquiries in
              one simple, fast interface.
            </p>
            <p className="text-sm md:text-base text-slate-300">
              Use the sidebar on the left to jump into the dashboard, properties, rentals,
              inquiries, and user management. As an admin, you can invite new team members
              and keep everything organized.
            </p>
            <div className="flex flex-wrap gap-3 mt-4">
              <a
                href={user ? "/dashboard" : "/auth"}
                className="inline-flex items-center px-4 py-2 rounded-md bg-blue-600 hover:bg-blue-700 text-sm font-medium"
              >
                {user ? "Go to dashboard" : "Log in to get started"}
              </a>
              {!user && (
                <a
                  href="/signup"
                  className="inline-flex items-center px-4 py-2 rounded-md border border-slate-600 text-sm font-medium hover:bg-slate-800"
                >
                  Create a basic user account
                </a>
              )}
            </div>
          </div>

          <div className="relative">
            <div className="absolute -inset-4 bg-blue-500/20 blur-3xl rounded-full" />
            <div className="relative rounded-2xl border border-slate-700 bg-slate-900/70 p-4 space-y-3 shadow-xl">
              <div className="flex items-center justify-between text-xs text-slate-400">
                <span>MiniRent overview</span>
                <span>Live preview</span>
              </div>
              <div className="grid grid-cols-3 gap-3 text-xs">
                <div className="rounded-lg bg-slate-800/80 p-3 space-y-1">
                  <div className="text-slate-400">Properties</div>
                  <div className="text-xl font-semibold text-blue-300">24</div>
                  <div className="text-[11px] text-emerald-400">+3 this month</div>
                </div>
                <div className="rounded-lg bg-slate-800/80 p-3 space-y-1">
                  <div className="text-slate-400">Active rentals</div>
                  <div className="text-xl font-semibold text-emerald-300">12</div>
                  <div className="text-[11px] text-emerald-400">98% occupancy</div>
                </div>
                <div className="rounded-lg bg-slate-800/80 p-3 space-y-1">
                  <div className="text-slate-400">New inquiries</div>
                  <div className="text-xl font-semibold text-amber-300">5</div>
                  <div className="text-[11px] text-amber-400">Last 7 days</div>
                </div>
              </div>
              <p className="text-[11px] text-slate-400">
                These are sample numbers to give you a feel for the dashboard. Once you
                start adding properties and rentals, this overview will reflect your own
                data.
              </p>
            </div>
          </div>
        </section>

        <section className="grid gap-8 md:grid-cols-3 text-sm text-slate-200">
          <div className="space-y-2">
            <h2 className="font-semibold text-white">For small teams</h2>
            <p className="text-slate-300">
              MiniRent is designed for solo landlords and small agencies who want clarity
              without the complexity of large enterprise systems.
            </p>
          </div>
          <div className="space-y-2">
            <h2 className="font-semibold text-white">Everything in one place</h2>
            <p className="text-slate-300">
              Keep an eye on properties, tenants, leases, and inquiries from a single
              dashboard, with quick access via the sidebar.
            </p>
          </div>
          <div className="space-y-2">
            <h2 className="font-semibold text-white">Built to extend</h2>
            <p className="text-slate-300">
              Because it uses ASP.NET Core, PostgreSQL, and Next.js, you can easily add
              new features like reports, reminders, or integrations as your needs grow.
            </p>
          </div>
        </section>
      </div>
    </div>
  );
}