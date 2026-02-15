"use client"

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { useToast } from "@/hooks/use-toast";

export default function RentalsPage() {
    const [rentals, setRentals] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [filter, setFilter] = useState<"all" | "my">("all");
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [currentUserId, setCurrentUserId] = useState<number | null>(null);
    const [highlightedId, setHighlightedId] = useState<number | null>(null);
    const { toast } = useToast();

    useEffect(() => {
        if (typeof (window as any) === "undefined") return;

        const token = (window as any).localStorage.getItem("token");
        setIsLoggedIn(!!token);

        if (token) {
            try {
                const base64Url = token.split('.')[1];
                const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                const jsonPayload = decodeURIComponent((window as any).atob(base64).split('').map(function (c: string) {
                    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                }).join(''));
                const payload = JSON.parse(jsonPayload);
                if (payload.nameid) setCurrentUserId(parseInt(payload.nameid));

                const userStr = (window as any).localStorage.getItem("user");
                if (userStr) {
                    const user = JSON.parse(userStr);
                    if (user.id) setCurrentUserId(user.id);
                }
            } catch (e) {
                console.error("Error parsing token", e);
            }
        }

        // Check for highlighting
        const params = new URLSearchParams((window as any).location.search);
        const idParam = params.get("id");
        if (idParam) {
            setHighlightedId(parseInt(idParam));
            setTimeout(() => setHighlightedId(null), 3000);
        }

        fetchRentals();
    }, [filter]);

    const fetchRentals = () => {
        const token = (window as any).localStorage.getItem("token");
        const query = filter === "my" ? "?mode=my" : "";
        setLoading(true);
        fetch(`/api/rentals${query}`, {
            headers: token ? { "Authorization": `Bearer ${token}` } : {}
        })
            .then(async (res) => {
                if (res.status === 401) {
                    if (typeof (window as any) !== "undefined") {
                        (window as any).location.href = "/auth?reason=login-required";
                    }
                    return;
                }
                if (!res.ok) throw new Error("Failed to fetch rentals");
                const json: any = await res.json();
                const data = Array.isArray(json) ? json : json.data;
                setRentals(data || []);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    };

    const handleStatusUpdate = async (id: number, status: string) => {
        try {
            const token = (window as any).localStorage.getItem("token");
            const res = await fetch(`/api/rentals/${id}/status`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ status })
            });

            if (res.ok) {
                toast({
                    title: "Success",
                    description: `Rental status updated to ${status}`,
                });
                fetchRentals();
            } else {
                const data: any = await res.json();
                toast({
                    title: "Error",
                    description: data.error || "Failed to update status",
                    variant: "destructive",
                });
            }
        } catch (err) {
            console.error("Failed to update status:", err);
            toast({
                title: "Error",
                description: "An error occurred while updating status",
                variant: "destructive",
            });
        }
    };

    return (
        <div className="p-8">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-2xl font-bold">Rentals</h1>
                <div className="flex gap-4">
                    {isLoggedIn && (
                        <div className="flex bg-muted rounded-lg p-1">
                            <button
                                className={`px-4 py-2 rounded-md text-sm font-medium transition-colors ${filter === "all" ? "bg-background shadow text-foreground" : "text-muted-foreground hover:text-foreground"}`}
                                onClick={() => setFilter("all")}
                            >
                                All Rentals
                            </button>
                            <button
                                className={`px-4 py-2 rounded-md text-sm font-medium transition-colors ${filter === "my" ? "bg-background shadow text-foreground" : "text-muted-foreground hover:text-foreground"}`}
                                onClick={() => setFilter("my")}
                            >
                                My Rentals
                            </button>
                        </div>
                    )}
                </div>
            </div>
            {loading ? (
                <div>Loading...</div>
            ) : error ? (
                <div className="text-red-500">{error}</div>
            ) : (
                Array.isArray(rentals) && rentals.length === 0 ? (
                    <div className="text-muted-foreground">No rentals yet. Add one to get started.</div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {Array.isArray(rentals) && rentals.map((rental) => (
                            <Card
                                key={rental.id}
                                className={cn(
                                    "p-4 flex flex-col justify-between transition-all duration-500",
                                    highlightedId === rental.id && "border-primary border-2 shadow-lg ring-2 ring-primary/20 scale-[1.02]"
                                )}
                                id={`rental-${rental.id}`}
                            >
                                <div>
                                    <div className="flex justify-between items-start mb-2">
                                        <div className="font-semibold text-lg">{rental.propertyAddress}</div>
                                        <div className={`text-[10px] px-2 py-0.5 rounded-full uppercase font-bold ${rental.status === "Active" ? "bg-green-100 text-green-700" :
                                            rental.status === "Pending" ? "bg-yellow-100 text-yellow-700" :
                                                rental.status === "Rejected" ? "bg-red-100 text-red-700" :
                                                    "bg-gray-100 text-gray-700"
                                            }`}>
                                            {rental.status}
                                        </div>
                                    </div>
                                    <div className="text-muted-foreground text-sm">Tenant: {rental.tenantName}</div>
                                    <div className="text-muted-foreground text-sm">Rent: ${rental.monthlyRent}</div>
                                    <div className="text-muted-foreground text-xs mt-2 italic flex justify-between">
                                        <span>Owner: {rental.createdBy}</span>
                                        <span>ID: {rental.id}</span>
                                    </div>
                                </div>

                                {rental.status === "Pending" && currentUserId && rental.ownerId === currentUserId && (
                                    <div className="flex gap-2 mt-4">
                                        <Button
                                            size="sm"
                                            className="flex-1 bg-green-600 hover:bg-green-700"
                                            onClick={() => handleStatusUpdate(rental.id, "Active")}
                                        >
                                            Approve
                                        </Button>
                                        <Button
                                            size="sm"
                                            variant="destructive"
                                            className="flex-1"
                                            onClick={() => handleStatusUpdate(rental.id, "Rejected")}
                                        >
                                            Reject
                                        </Button>
                                    </div>
                                )}
                            </Card>
                        ))}
                    </div>
                )
            )}
        </div>
    );
}
