"use client";

import { useEffect, useState, Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import Link from "next/link";
import {
    Loader2,
    Search,
    Building2,
    MessageSquare,
    Key,
    ArrowRight,
    Info
} from "lucide-react";

function SearchContent() {
    const searchParams = useSearchParams();
    const query = searchParams.get("q") || "";
    const [results, setResults] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        if (query) {
            fetchResults();
        }
    }, [query]);

    const fetchResults = async () => {
        setLoading(true);
        setError("");
        try {
            const token = localStorage.getItem("token");
            const res = await fetch(`/api/search?q=${encodeURIComponent(query)}`, {
                headers: token ? { "Authorization": `Bearer ${token}` } : {}
            });

            if (!res.ok) throw new Error("Search failed");
            const data = await res.json() as any;
            setResults(data);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const getIcon = (type: string) => {
        switch (type) {
            case "Property": return <Building2 className="w-5 h-5 text-blue-500" />;
            case "Inquiry": return <MessageSquare className="w-5 h-5 text-green-500" />;
            case "Rental": return <Key className="w-5 h-5 text-purple-500" />;
            default: return <Search className="w-5 h-5 text-slate-400" />;
        }
    };

    return (
        <div className="p-8 max-w-5xl mx-auto">
            <div className="flex items-center gap-3 mb-8">
                <div className="p-3 bg-slate-100 rounded-full">
                    <Search className="w-6 h-6 text-slate-600" />
                </div>
                <div>
                    <h1 className="text-3xl font-bold tracking-tight text-slate-900">Search Results</h1>
                    <p className="text-slate-500 text-sm">Found {results.length} results for "{query}"</p>
                </div>
            </div>

            {loading ? (
                <div className="flex flex-col items-center justify-center py-20 gap-4">
                    <Loader2 className="w-10 h-10 animate-spin text-blue-600" />
                    <p className="text-slate-500 font-medium">Searching across MiniRent...</p>
                </div>
            ) : error ? (
                <div className="bg-red-50 border border-red-200 text-red-700 p-4 rounded-lg flex items-center gap-3">
                    <div className="p-2 bg-red-100 rounded-full">
                        <Info className="w-4 h-4" />
                    </div>
                    <p>{error}</p>
                </div>
            ) : results.length === 0 ? (
                <div className="text-center py-20 bg-slate-50 rounded-2xl border-2 border-dashed border-slate-200">
                    <div className="w-16 h-16 bg-white rounded-2xl shadow-sm flex items-center justify-center mx-auto mb-4">
                        <Search className="w-8 h-8 text-slate-300" />
                    </div>
                    <h3 className="text-lg font-semibold text-slate-900">No results found</h3>
                    <p className="text-slate-500 max-w-xs mx-auto mt-2">
                        Try adjusting your search terms or check for typos.
                    </p>
                </div>
            ) : (
                <div className="grid gap-4">
                    {results.map((result, index) => (
                        <Link key={`${result.type}-${result.id}-${index}`} href={result.url}>
                            <Card className="p-5 hover:shadow-md transition-all hover:border-blue-200 group relative">
                                <div className="flex items-start gap-4">
                                    <div className="p-3 bg-slate-50 rounded-xl group-hover:bg-blue-50 transition-colors">
                                        {getIcon(result.type)}
                                    </div>
                                    <div className="flex-1 min-w-0">
                                        <div className="flex items-center gap-2 mb-1">
                                            <Badge variant="outline" className="text-[10px] uppercase font-bold tracking-wider">
                                                {result.type}
                                            </Badge>
                                            <h3 className="text-lg font-bold text-slate-900 truncate">
                                                {result.title}
                                            </h3>
                                        </div>
                                        <p className="text-slate-500 text-sm truncate">
                                            {result.subtitle}
                                        </p>
                                    </div>
                                    <div className="self-center">
                                        <ArrowRight className="w-5 h-5 text-slate-300 group-hover:text-blue-500 group-hover:translate-x-1 transition-all" />
                                    </div>
                                </div>
                            </Card>
                        </Link>
                    ))}
                </div>
            )}
        </div>
    );
}

export default function SearchPage() {
    return (
        <Suspense fallback={
            <div className="flex items-center justify-center min-h-screen">
                <Loader2 className="w-10 h-10 animate-spin text-blue-600" />
            </div>
        }>
            <SearchContent />
        </Suspense>
    );
}
