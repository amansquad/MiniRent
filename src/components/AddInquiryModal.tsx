"use client";

import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { useToast } from "@/hooks/use-toast";

interface AddInquiryModalProps {
    isOpen: boolean;
    onClose: () => void;
    propertyId: number;
    propertyAddress: string;
}

export default function AddInquiryModal({ isOpen, onClose, propertyId, propertyAddress }: AddInquiryModalProps) {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [phone, setPhone] = useState("");
    const [message, setMessage] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const { toast } = useToast();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError("");

        try {
            const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;
            const res = await fetch("/api/inquiries", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify({
                    propertyId,
                    name,
                    email,
                    phone,
                    message
                })
            });

            if (!res.ok) {
                const data = await res.json();
                throw new Error(data.message || "Failed to send inquiry");
            }

            onClose();
            setName("");
            setEmail("");
            setPhone("");
            setMessage("");
            toast({
                title: "Inquiry Sent",
                description: "Your inquiry has been sent successfully!",
            });
        } catch (err: any) {
            setError(err.message);
            toast({
                title: "Error",
                description: err.message || "Failed to send inquiry",
                variant: "destructive",
            });
        } finally {
            setLoading(false);
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Inquire about {propertyAddress}</DialogTitle>
                </DialogHeader>
                <form onSubmit={handleSubmit} className="grid gap-4 py-4">
                    {error && <div className="text-red-500 text-sm">{error}</div>}

                    <div className="grid gap-2">
                        <Label htmlFor="name">Name</Label>
                        <Input
                            id="name"
                            value={name}
                            onChange={(e: any) => setName(e.target.value)}
                            required
                        />
                    </div>

                    <div className="grid gap-2">
                        <Label htmlFor="email">Email</Label>
                        <Input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e: any) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="grid gap-2">
                        <Label htmlFor="phone">Phone</Label>
                        <Input
                            id="phone"
                            value={phone}
                            onChange={(e: any) => {
                                const val = e.target.value.replace(/\D/g, '').slice(0, 10);
                                setPhone(val);
                            }}
                            maxLength={10}
                            placeholder="10 digits number"
                            required
                        />
                    </div>

                    <div className="grid gap-2">
                        <Label htmlFor="message">Message</Label>
                        <Textarea
                            id="message"
                            value={message}
                            onChange={(e: any) => setMessage(e.target.value)}
                        />
                    </div>

                    <div className="flex justify-end gap-4 mt-4">
                        <Button type="button" variant="outline" onClick={onClose}>
                            Cancel
                        </Button>
                        <Button type="submit" disabled={loading}>
                            {loading ? "Sending..." : "Send Inquiry"}
                        </Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}
