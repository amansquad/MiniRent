"use client"

import { useEffect, useState, useCallback } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Loader2, MessageSquareReply, Send } from "lucide-react";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import { useToast } from "@/hooks/use-toast";
import { getUser } from "@/lib/auth";

export default function InquiriesPage() {
    const [inquiries, setInquiries] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const { toast } = useToast();
    const [currentUserId, setCurrentUserId] = useState<number | null>(null);
    const [replies, setReplies] = useState<{ [key: number]: string }>({});
    const [submitting, setSubmitting] = useState<{ [key: number]: boolean }>({});
    const [highlightedId, setHighlightedId] = useState<number | null>(null);

    const fetchInquiries = useCallback(async () => {
        setLoading(true);
        try {
            const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;
            const res = await fetch("/api/inquiries", {
                headers: token ? { "Authorization": `Bearer ${token}` } : {}
            });

            if (res.status === 401) {
                if (typeof (window as any) !== "undefined") {
                    (window as any).location.href = "/auth?reason=login-required";
                }
                return;
            }

            if (!res.ok) throw new Error("Failed to fetch inquiries");
            const json: any = await res.json();
            const data = Array.isArray(json) ? json : json.data;
            if (!Array.isArray(data)) {
                throw new Error("Unexpected inquiries response format");
            }
            setInquiries(data);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        if (typeof (window as any) === "undefined") return;

        // Get current user ID
        const user = getUser();
        if (user) {
            setCurrentUserId(user.id);
        }

        // Check for highlighting
        const params = new URLSearchParams((window as any).location.search);
        const idParam = params.get("id");
        if (idParam) {
            setHighlightedId(parseInt(idParam));
            setTimeout(() => setHighlightedId(null), 3000);

            // Clear the param
            (window as any).history.replaceState({}, "", "/inquiries");
        }

        fetchInquiries();
    }, [fetchInquiries]);

    const handleStatusUpdate = async (id: number, status: string, ownerReply?: string) => {
        setSubmitting(prev => ({ ...prev, [id]: true }));
        try {
            const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;
            const res = await fetch(`/api/inquiries/${id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify({ status, ownerReply })
            });

            if (!res.ok) throw new Error("Failed to update inquiry");

            // Refresh list
            fetchInquiries();
            toast({
                title: "Success",
                description: "Inquiry updated successfully",
            });
            if (ownerReply) {
                setReplies(prev => ({ ...prev, [id]: "" }));
            }
        } catch (err: any) {
            toast({
                title: "Error",
                description: err.message || "Failed to update inquiry",
                variant: "destructive",
            });
        } finally {
            setSubmitting(prev => ({ ...prev, [id]: false }));
        }
    };

    const getStatusColor = (status: string) => {
        switch ((status || "").toLowerCase()) {
            case 'new': return 'bg-blue-500';
            case 'accepted': return 'bg-green-500';
            case 'rejected': return 'bg-red-500';
            default: return 'bg-gray-500';
        }
    };

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-8">Inquiries</h1>
            {loading ? (
                <div className="flex justify-center p-12">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                </div>
            ) : error ? (
                <div className="text-red-500 bg-red-50 p-4 rounded-md">{error}</div>
            ) : (
                inquiries.length === 0 ? (
                    <div className="text-center py-12 border-2 border-dashed rounded-lg bg-muted/20">
                        <p className="text-muted-foreground">No inquiries yet. Once people contact you about your properties, they will appear here.</p>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {inquiries.map((inquiry) => (
                            <Card
                                key={inquiry.id}
                                className={cn(
                                    "p-5 flex flex-col gap-4 transition-all duration-500",
                                    highlightedId === inquiry.id && "border-primary border-2 shadow-lg ring-2 ring-primary/20 scale-[1.02]"
                                )}
                                id={`inquiry-${inquiry.id}`}
                            >
                                <div className="flex justify-between items-start">
                                    <div>
                                        <h3 className="font-semibold text-lg">{inquiry.name}</h3>
                                        <p className="text-sm text-muted-foreground">{inquiry.email}</p>
                                        <p className="text-sm text-muted-foreground">{inquiry.phone}</p>
                                    </div>
                                    <Badge className={getStatusColor(inquiry.status)}>{inquiry.status}</Badge>
                                </div>

                                <div>
                                    <p className="text-xs font-medium uppercase text-muted-foreground">About Property</p>
                                    <p className="font-medium text-primary">{inquiry.propertyAddress || "General Inquiry"}</p>
                                </div>

                                {inquiry.message && (
                                    <div className="bg-muted/50 p-3 rounded text-sm italic border-l-2 border-primary/30">
                                        <p className="font-semibold text-[10px] uppercase text-muted-foreground not-italic mb-1">Inquiry Message</p>
                                        "{inquiry.message}"
                                    </div>
                                )}

                                {inquiry.ownerReply && (
                                    <div className="bg-primary/5 p-3 rounded text-sm border-l-2 border-primary">
                                        <p className="font-semibold text-[10px] uppercase text-primary mb-1">Owner Response</p>
                                        {inquiry.ownerReply}
                                    </div>
                                )}

                                <div className="flex justify-between items-center text-xs text-muted-foreground border-t pt-2 mt-auto">
                                    <span>{new Date(inquiry.createdAt).toLocaleDateString()}</span>
                                    <span>By: {inquiry.createdBy || "Guest"}</span>
                                </div>

                                {currentUserId === inquiry.ownerId && (
                                    <div className="space-y-4 pt-4 border-t">
                                        <div className="space-y-2">
                                            <div className="flex justify-between items-center">
                                                <Label htmlFor={`reply-${inquiry.id}`} className="text-xs font-bold text-primary">
                                                    {inquiry.ownerReply ? "Update Response" : "Quick Reply"}
                                                </Label>
                                                {inquiry.ownerReply && (
                                                    <Badge variant="outline" className="text-[9px]">Already Replied</Badge>
                                                )}
                                            </div>
                                            <Textarea
                                                id={`reply-${inquiry.id}`}
                                                placeholder="Type your response here..."
                                                className="min-h-[80px] text-sm bg-slate-50 border-slate-200 focus:bg-white transition-colors"
                                                value={replies[inquiry.id] !== undefined ? replies[inquiry.id] : (inquiry.ownerReply || "")}
                                                onChange={(e: any) => setReplies({ ...replies, [inquiry.id]: e.target.value })}
                                            />
                                        </div>
                                        <div className="flex gap-2">
                                            {inquiry.status.toLowerCase() === 'new' && (
                                                <>
                                                    <Button
                                                        size="sm"
                                                        className="flex-1 bg-green-600 hover:bg-green-700"
                                                        disabled={submitting[inquiry.id]}
                                                        onClick={() => handleStatusUpdate(inquiry.id, "Accepted", replies[inquiry.id] !== undefined ? replies[inquiry.id] : (inquiry.ownerReply || ""))}
                                                    >
                                                        {submitting[inquiry.id] ? <Loader2 className="w-4 h-4 animate-spin" /> : "Accept"}
                                                    </Button>
                                                    <Button
                                                        size="sm"
                                                        variant="destructive"
                                                        className="flex-1"
                                                        disabled={submitting[inquiry.id]}
                                                        onClick={() => handleStatusUpdate(inquiry.id, "Rejected", replies[inquiry.id] !== undefined ? replies[inquiry.id] : (inquiry.ownerReply || ""))}
                                                    >
                                                        {submitting[inquiry.id] ? <Loader2 className="w-4 h-4 animate-spin" /> : "Reject"}
                                                    </Button>
                                                </>
                                            )}
                                            {inquiry.status.toLowerCase() !== 'new' && (
                                                <Button
                                                    size="sm"
                                                    className="w-full"
                                                    disabled={submitting[inquiry.id] || !replies[inquiry.id]}
                                                    onClick={() => handleStatusUpdate(inquiry.id, inquiry.status, replies[inquiry.id])}
                                                >
                                                    {submitting[inquiry.id] ? <Loader2 className="w-4 h-4 animate-spin" /> : "Update Response Only"}
                                                </Button>
                                            )}
                                        </div>
                                    </div>
                                )}

                                {inquiry.status.toLowerCase() === 'accepted' && currentUserId === inquiry.createdById && (
                                    <div className="pt-2">
                                        <Button
                                            size="sm"
                                            className="w-full bg-primary"
                                            onClick={() => {
                                                if (typeof (window as any) !== "undefined") {
                                                    (window as any).location.href = `/properties?rent=${inquiry.propertyId}`;
                                                }
                                            }}
                                        >
                                            Proceed to Rental
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
