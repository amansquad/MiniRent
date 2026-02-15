"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";

export function SearchBar() {
    const [query, setQuery] = useState("");
    const router = useRouter();

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        if (query.trim()) {
            router.push(`/search?q=${encodeURIComponent(query.trim())}`);
        }
    };

    return (
        <form onSubmit={handleSearch} className="relative w-full px-2 mb-6">
            <Search className="absolute left-5 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
            <Input
                type="search"
                placeholder="Global search..."
                className="pl-9 h-10 bg-slate-100 dark:bg-slate-800 border-none focus-visible:ring-1 focus-visible:ring-blue-500"
                value={query}
                onChange={(e: any) => setQuery(e.target.value)}
            />
        </form>
    );
}
