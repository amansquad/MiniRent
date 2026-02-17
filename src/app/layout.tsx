"use client";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { Toaster } from "@/components/ui/toaster";
import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  Home,
  LayoutDashboard,
  Building2,
  Key,
  MessageSquare,
  Users,
  User,
  Info,
  LogOut,
  LogIn
} from "lucide-react";


const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  subsets: ["latin"],
  variable: "--font-geist-mono",
  weight: ["400", "700"],
});


import { Button } from "@/components/ui/button";
import { SearchBar } from "@/components/SearchBar";
import { MobileNav } from "@/components/MobileNav";
import { getUser, logout } from "@/lib/auth";
import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";

export default function RootLayout({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<any>(null);
  const pathname = usePathname();

  useEffect(() => {
    setUser(getUser());
  }, [pathname]);

  const navItems = [
    { href: "/", label: "Home", icon: Home },
    { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
    { href: "/properties", label: "Properties", icon: Building2 },
    { href: "/rentals", label: "Rentals", icon: Key },
    { href: "/inquiries", label: "Inquiries", icon: MessageSquare },
    ...(user?.role === "Admin" ? [{ href: "/users", label: "Users", icon: Users }] : []),
    { href: "/profile", label: "Profile", icon: User },
    { href: "/about", label: "About Us", icon: Info },
  ];

  return (
    <html lang="en" className={geistMono.variable}>
      <body className={geistMono.className}>
        <div className="flex min-h-screen">
          {/* Sidebar - Desktop only */}
          <aside className="w-64 bg-slate-50 dark:bg-slate-900 border-r hidden md:flex flex-col p-6 sticky top-0 h-screen">
            <div className="flex items-center gap-2 mb-8 px-2">
              <div className="p-2 bg-blue-600 rounded-lg">
                <Building2 className="w-5 h-5 text-white" />
              </div>
              <h2 className="text-xl font-bold tracking-tight text-slate-900 dark:text-white">MiniRent</h2>
            </div>

            <SearchBar />

            <nav className="flex-1 flex flex-col gap-1">
              {navItems.map((item) => {
                const Icon = item.icon;
                const isActive = pathname === item.href;

                return (
                  <Link
                    key={item.href}
                    href={item.href}
                    className="w-full"
                  >
                    <Button
                      variant={isActive ? "secondary" : "ghost"}
                      className={cn(
                        "w-full justify-start gap-3 transition-all",
                        isActive
                          ? "bg-blue-50 text-blue-700 hover:bg-blue-100 hover:text-blue-800 dark:bg-blue-900/20 dark:text-blue-400 font-semibold"
                          : "text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800"
                      )}
                    >
                      <Icon className={cn("w-4 h-4", isActive ? "text-blue-600 dark:text-blue-400" : "text-slate-400")} />
                      {item.label}
                    </Button>
                  </Link>
                );
              })}
            </nav>

            <div className="pt-6 border-t mt-auto">
              {user ? (
                <div className="space-y-4">
                  <div className="px-2">
                    <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2">Account</p>
                    <p className="text-sm font-medium text-slate-900 dark:text-white truncate">{user.fullName}</p>
                    <p className="text-xs text-slate-500">{user.role}</p>
                  </div>
                  <Button
                    variant="ghost"
                    className="w-full justify-start gap-3 text-red-600 hover:text-red-700 hover:bg-red-50 dark:hover:bg-red-900/10 transition-colors"
                    onClick={logout}
                  >
                    <LogOut className="w-4 h-4" />
                    Logout
                  </Button>
                </div>
              ) : (
                <Link href="/auth" className="w-full">
                  <Button variant="default" className="w-full gap-2 bg-blue-600 hover:bg-blue-700 transition-colors">
                    <LogIn className="w-4 h-4" />
                    Login
                  </Button>
                </Link>
              )}
            </div>
          </aside>

          {/* Main content */}
          <main className="flex-1 bg-white dark:bg-slate-950 overflow-auto">
            {/* Mobile header */}
            <header className="md:hidden sticky top-0 z-40 bg-white dark:bg-slate-950 border-b px-4 py-3 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <MobileNav user={user} navItems={navItems} />
                <div className="flex items-center gap-2">
                  <div className="p-1.5 bg-blue-600 rounded-lg">
                    <Building2 className="w-4 h-4 text-white" />
                  </div>
                  <h2 className="text-lg font-bold tracking-tight text-slate-900 dark:text-white">MiniRent</h2>
                </div>
              </div>
              <div className="flex items-center gap-2">
                <SearchBar />
              </div>
            </header>

            <div className="min-h-full">
              {children}
            </div>
          </main>
        </div>
        <Toaster />
      </body>
    </html>
  );
}