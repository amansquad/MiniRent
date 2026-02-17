"use client";

import { useState } from "react";
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
    LogIn,
    Menu,
    X
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";
import { cn } from "@/lib/utils";
import { logout } from "@/lib/auth";

interface MobileNavProps {
    user: any;
    navItems: Array<{
        href: string;
        label: string;
        icon: any;
    }>;
}

export function MobileNav({ user, navItems }: MobileNavProps) {
    const [open, setOpen] = useState(false);
    const pathname = usePathname();

    const handleLogout = () => {
        logout();
        setOpen(false);
    };

    return (
        <Sheet open={open} onOpenChange={setOpen}>
            <SheetTrigger asChild>
                <Button variant="ghost" size="icon" className="md:hidden">
                    <Menu className="h-6 w-6" />
                    <span className="sr-only">Toggle menu</span>
                </Button>
            </SheetTrigger>
            <SheetContent side="left" className="w-64 p-0">
                <div className="flex flex-col h-full">
                    <SheetHeader className="p-6 border-b">
                        <SheetTitle className="flex items-center gap-2">
                            <div className="p-2 bg-blue-600 rounded-lg">
                                <Building2 className="w-5 h-5 text-white" />
                            </div>
                            <span className="text-xl font-bold">MiniRent</span>
                        </SheetTitle>
                    </SheetHeader>

                    <nav className="flex-1 flex flex-col gap-1 p-4">
                        {navItems.map((item) => {
                            const Icon = item.icon;
                            const isActive = pathname === item.href;

                            return (
                                <Link
                                    key={item.href}
                                    href={item.href}
                                    onClick={() => setOpen(false)}
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

                    <div className="p-4 border-t mt-auto">
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
                                    onClick={handleLogout}
                                >
                                    <LogOut className="w-4 h-4" />
                                    Logout
                                </Button>
                            </div>
                        ) : (
                            <Link href="/auth" onClick={() => setOpen(false)} className="w-full">
                                <Button variant="default" className="w-full gap-2 bg-blue-600 hover:bg-blue-700 transition-colors">
                                    <LogIn className="w-4 h-4" />
                                    Login
                                </Button>
                            </Link>
                        )}
                    </div>
                </div>
            </SheetContent>
        </Sheet>
    );
}
