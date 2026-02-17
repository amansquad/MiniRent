"use client"

import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Loader2 } from "lucide-react";
import { useToast } from "@/hooks/use-toast";

interface EndRentalModalProps {
    isOpen: boolean;
    onClose: () => void;
    rentalId: string;
    propertyAddress: string;
    onSuccess: () => void;
}

export function EndRentalModal({ isOpen, onClose, rentalId, propertyAddress, onSuccess }: EndRentalModalProps) {
    const { toast } = useToast();
    const [loading, setLoading] = useState(false);
    const [formData, setFormData] = useState({
        endDate: new Date().toISOString().split('T')[0],
        notes: ""
    });

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        try {
            const token = (typeof (window as any) !== "undefined") ? (window as any).localStorage.getItem("token") : null;
            const res = await fetch(`/api/rentals/${rentalId}/end`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify({
                    endDate: new Date(formData.endDate).toISOString(),
                    notes: formData.notes
                })
            });

            if (!res.ok) {
                const data = await res.json() as any;
                throw new Error(data.error || "Failed to end rental");
            }

            toast({
                title: "Success",
                description: "Rental ended successfully. Property is now Available.",
            });
            onSuccess();
            onClose();
        } catch (error: any) {
            toast({
                title: "Error",
                description: error.message,
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
                    <DialogTitle>End Rental</DialogTitle>
                </DialogHeader>
                <div className="mb-4">
                    <p className="text-sm font-medium text-muted-foreground">Property</p>
                    <p className="font-semibold">{propertyAddress}</p>
                </div>
                <form onSubmit={onSubmit} className="grid gap-4 py-4">
                    <div className="grid gap-2">
                        <Label htmlFor="endDate" className="text-xs uppercase font-bold text-muted-foreground">End Date</Label>
                        <Input
                            id="endDate"
                            type="date"
                            value={formData.endDate}
                            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setFormData(prev => ({ ...prev, endDate: e.target.value }))}
                            required
                        />
                    </div>
                    <div className="grid gap-2">
                        <Label htmlFor="notes" className="text-xs uppercase font-bold text-muted-foreground">Completion Notes</Label>
                        <Textarea
                            id="notes"
                            placeholder="Reason for ending, property condition, etc."
                            value={formData.notes}
                            onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
                        />
                    </div>
                    <DialogFooter className="mt-4">
                        <Button type="button" variant="ghost" onClick={onClose} disabled={loading}>Cancel</Button>
                        <Button type="submit" variant="destructive" disabled={loading}>
                            {loading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                            End Rental
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    );
}
