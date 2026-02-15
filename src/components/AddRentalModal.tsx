"use client"

import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Loader2 } from "lucide-react";

interface Property {
    id: number;
    address: string;
    monthlyRent: number;
}

interface AddRentalModalProps {
    isOpen: boolean;
    onClose: () => void;
    property: Property | null;
}

export function AddRentalModal({ isOpen, onClose, property }: AddRentalModalProps) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [formData, setFormData] = useState({
        tenantName: "",
        tenantPhone: "",
        tenantEmail: "",
        startDate: tomorrowStr,
        deposit: "",
        notes: ""
    });

    const handleChange = (e: any) => {
        const { id, value } = e.target;
        setFormData(prev => ({ ...prev, [id]: value }));
    };

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!property) return;

        setLoading(true);
        setError("");
        const token = typeof (window as any) !== "undefined" ? ((window as any).localStorage.getItem)("token") : null;

        try {
            const payload = {
                propertyId: property.id,
                tenantName: formData.tenantName,
                tenantPhone: formData.tenantPhone,
                tenantEmail: formData.tenantEmail || null,
                startDate: new Date(formData.startDate).toISOString(),
                deposit: Number(formData.deposit),
                monthlyRent: property.monthlyRent,
                notes: formData.notes
            };

            const res = await fetch("/api/rentals", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const errorData: any = await res.json().catch(() => ({}));
                throw new Error(errorData.error || "Failed to submit rental request");
            }

            onClose();
            if (typeof (window as any) !== "undefined") {
                (window as any).alert("Rental request submitted successfully! The owner will review it.");
            }
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Request to Rent</DialogTitle>
                </DialogHeader>
                <div className="mb-4 p-3 bg-muted rounded-md text-sm">
                    <p className="font-semibold">{property?.address}</p>
                    <p className="text-muted-foreground">${property?.monthlyRent}/month</p>
                </div>
                <form onSubmit={onSubmit} className="grid gap-4 py-4">
                    {error && <div className="text-red-500 text-sm p-2 bg-red-50 rounded">{error}</div>}

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="tenantName" className="text-right text-xs">Full Name</Label>
                        <Input id="tenantName" value={formData.tenantName} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="tenantPhone" className="text-right text-xs">Phone</Label>
                        <Input
                            id="tenantPhone"
                            value={formData.tenantPhone}
                            onChange={(e: any) => {
                                const val = e.target.value.replace(/\D/g, '').slice(0, 10);
                                setFormData(prev => ({ ...prev, tenantPhone: val }));
                            }}
                            maxLength={10}
                            placeholder="10 digits"
                            className="col-span-3"
                            required
                        />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="tenantEmail" className="text-right text-xs">Email</Label>
                        <Input id="tenantEmail" type="email" value={formData.tenantEmail} onChange={handleChange} className="col-span-3" />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="startDate" className="text-right text-xs">Start Date</Label>
                        <Input
                            id="startDate"
                            type="date"
                            value={formData.startDate}
                            onChange={handleChange}
                            className="col-span-3"
                            required
                            min={tomorrowStr}
                        />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="deposit" className="text-right text-xs">Deposit ($)</Label>
                        <Input id="deposit" type="number" value={formData.deposit} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="notes" className="text-right text-xs">Notes</Label>
                        <Textarea id="notes" value={formData.notes} onChange={handleChange} className="col-span-3" placeholder="Optional messages to the owner..." />
                    </div>

                    <div className="flex justify-end gap-2">
                        <Button type="button" variant="outline" onClick={onClose} disabled={loading}>Cancel</Button>
                        <Button type="submit" disabled={loading}>
                            {loading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : "Submit Request"}
                        </Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}
