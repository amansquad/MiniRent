"use client"

import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

export function AddPropertyModal({ isOpen, onClose }: { isOpen: boolean; onClose: () => void }) {
    const [error, setError] = useState("");

    // Using simple state for form inputs to avoid external dependency issues if react-hook-form isn't installed
    // (though it likely is in a Next.js project, sticking to basics is safer for a quick fix)
    const [formData, setFormData] = useState({
        address: "",
        area: "",
        bedrooms: "",
        bathrooms: "",
        floor: "",
        monthlyRent: "",
        description: ""
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const target = e.target as any;
        const { id, value } = target;
        setFormData(prev => ({ ...prev, [id]: value }));
    };

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        const token = localStorage.getItem("token");

        try {
            const payload = {
                ...formData,
                area: Number(formData.area),
                bedrooms: Number(formData.bedrooms),
                bathrooms: Number(formData.bathrooms),
                floor: formData.floor ? Number(formData.floor) : null,
                monthlyRent: Number(formData.monthlyRent)
            };

            const res = await fetch("/api/properties", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const errorData: any = await res.json();
                throw new Error(errorData.error || "Failed to create property");
            }

            // Reset form
            setFormData({
                address: "",
                area: "",
                bedrooms: "",
                bathrooms: "",
                floor: "",
                monthlyRent: "",
                description: ""
            });
            onClose();
        } catch (err: any) {
            setError(err.message);
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Add New Property</DialogTitle>
                </DialogHeader>
                <form onSubmit={onSubmit} className="grid gap-4 py-4">
                    {error && <div className="text-red-500 text-sm">{error}</div>}

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="address" className="text-right">
                            Address
                        </Label>
                        <Input id="address" value={formData.address} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="area" className="text-right">
                            Area (m²)
                        </Label>
                        <Input id="area" type="number" value={formData.area} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="bedrooms" className="text-right">
                            Bedrooms
                        </Label>
                        <Input id="bedrooms" type="number" value={formData.bedrooms} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="bathrooms" className="text-right">
                            Bathrooms
                        </Label>
                        <Input id="bathrooms" type="number" step="0.5" value={formData.bathrooms} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="floor" className="text-right">
                            Floor
                        </Label>
                        <Input id="floor" type="number" value={formData.floor} onChange={handleChange} className="col-span-3" />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="monthlyRent" className="text-right">
                            Rent
                        </Label>
                        <Input id="monthlyRent" type="number" value={formData.monthlyRent} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="description" className="text-right">
                            Description
                        </Label>
                        <Input id="description" value={formData.description} onChange={handleChange} className="col-span-3" />
                    </div>

                    <div className="flex justify-end">
                        <Button type="submit">Save</Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}
