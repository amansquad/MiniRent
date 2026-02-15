"use client"

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { useToast } from "@/hooks/use-toast";
import { EndRentalModal } from "@/components/EndRentalModal";
import { Calendar, User, Phone, Mail, Info, CheckCircle2, XCircle, Ban, Clock } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import Link from "next/link";
import { getUser } from "@/lib/auth";

export default function RentalsPage() {
    const [rentals, setRentals] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [filter, setFilter] = useState<"all" | "my">("all");
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [currentUserId, setCurrentUserId] = useState<number | null>(null);
    const [highlightedId, setHighlightedId] = useState<number | null>(null);
    const [endRental, setEndRental] = useState<any | null>(null);
    const { toast } = useToast();

    useEffect(() => {
        const user = getUser();
        if (user) {
            setIsLoggedIn(true);
            setCurrentUserId(user.id);
        }

        // Check for highlighting
        if (typeof (window as any) !== "undefined") {
            const params = new URLSearchParams((window as any).location.search);
            const idParam = params.get("id");
            if (idParam) {
                setHighlightedId(parseInt(idParam));
                setTimeout(() => setHighlightedId(null), 3000);
            }
        }

        fetchRentals();
    }, [filter]);

    const fetchRentals = () => {
        const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;
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
            const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;
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
                                    "p-0 overflow-hidden flex flex-col transition-all duration-500",
                                    highlightedId === rental.id && "border-primary border-2 shadow-lg ring-2 ring-primary/20 scale-[1.02]"
                                )}
                                id={`rental-${rental.id}`}
                            >
                                <div className="p-5 flex-1 space-y-4">
                                    <div className="flex justify-between items-start">
                                        <div className="space-y-1">
                                            <h3 className="font-bold text-lg leading-tight">{rental.propertyAddress}</h3>
                                            <p className="text-primary font-medium flex items-center text-sm">
                                                <User className="w-3.5 h-3.5 mr-1.5" />
                                                {rental.tenantName}
                                            </p>
                                        </div>
                                        <Badge
                                            variant={rental.status === "Active" ? "default" : rental.status === "Pending" ? "secondary" : "outline"}
                                            className={cn(
                                                "uppercase text-[10px] h-5 px-1.5 font-bold tracking-wider",
                                                rental.status === "Active" && "bg-green-100 text-green-700 hover:bg-green-100",
                                                rental.status === "Rejected" && "bg-red-100 text-red-700 hover:bg-red-100"
                                            )}
                                        >
                                            {rental.status}
                                        </Badge>
                                    </div>

                                    <div className="grid grid-cols-2 gap-3 text-sm pt-2">
                                        <div className="bg-muted/50 p-2 rounded-md">
                                            <p className="text-[10px] text-muted-foreground uppercase font-bold mb-0.5">Monthly Rent</p>
                                            <p className="font-semibold">${rental.monthlyRent}</p>
                                        </div>
                                        <div className="bg-muted/50 p-2 rounded-md">
                                            <p className="text-[10px] text-muted-foreground uppercase font-bold mb-0.5">Start Date</p>
                                            <p className="font-semibold">{new Date(rental.startDate).toLocaleDateString()}</p>
                                        </div>
                                    </div>

                                    <div className="flex items-center justify-between text-xs text-muted-foreground border-t pt-3">
                                        <span className="flex items-center">
                                            <User className="w-3 h-3 mr-1" />
                                            {rental.createdBy}
                                        </span>
                                        <span>#{rental.id}</span>
                                    </div>
                                </div>

                                {/* Action Buttons Footer */}
                                <div className="p-3 bg-muted/20 border-t flex gap-2">
                                    {rental.status === "Pending" && currentUserId && rental.ownerId === currentUserId && (
                                        <>
                                            <Button
                                                size="sm"
                                                className="flex-1 h-8 text-xs bg-green-600 hover:bg-green-700"
                                                onClick={() => handleStatusUpdate(rental.id, "Active")}
                                            >
                                                <CheckCircle2 className="w-3 h-3 mr-1.5" />
                                                Approve
                                            </Button>
                                            <Button
                                                size="sm"
                                                variant="outline"
                                                className="flex-1 h-8 text-xs text-red-600 border-red-200 hover:bg-red-50"
                                                onClick={() => handleStatusUpdate(rental.id, "Rejected")}
                                            >
                                                <XCircle className="w-3 h-3 mr-1.5" />
                                                Reject
                                            </Button>
                                        </>
                                    )}

                                    {rental.status === "Active" && currentUserId && rental.ownerId === currentUserId && (
                                        <Button
                                            size="sm"
                                            variant="destructive"
                                            className="flex-1 h-8 text-xs"
                                            onClick={() => setEndRental(rental)}
                                        >
                                            <Ban className="w-3 h-3 mr-1.5" />
                                            End Rental
                                        </Button>
                                    )}

                                    <Button variant="ghost" size="sm" className="flex-1 h-8 text-xs" asChild>
                                        <Link href={`/properties/${rental.propertyId}`}>
                                            <Info className="w-3 h-3 mr-1.5" />
                                            Property
                                        </Link>
                                    </Button>
                                </div>
                            </Card>
                        ))}
                    </div>
                )
            )}

            {endRental && (
                <EndRentalModal
                    isOpen={!!endRental}
                    onClose={() => setEndRental(null)}
                    rentalId={endRental.id}
                    propertyAddress={endRental.propertyAddress}
                    onSuccess={fetchRentals}
                />
            )}
        </div>
    );
}
