"use client"

import { useState, useEffect } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

interface EditPropertyModalProps {
    isOpen: boolean;
    onClose: () => void;
    property: any;
}

export function EditPropertyModal({ isOpen, onClose, property }: EditPropertyModalProps) {
    const [error, setError] = useState("");
    const [formData, setFormData] = useState({
        address: "",
        area: "",
        bedrooms: "",
        bathrooms: "",
        floor: "",
        monthlyRent: "",
        description: "",
        status: "",
        title: "",
        city: "",
        state: "",
        zipCode: "",
        country: "",
        propertyType: ""
    });

    useEffect(() => {
        if (property) {
            setFormData({
                address: property.address || "",
                area: property.area?.toString() || "",
                bedrooms: property.bedrooms?.toString() || "",
                bathrooms: property.bathrooms?.toString() || "",
                floor: property.floor?.toString() || "",
                monthlyRent: property.monthlyRent?.toString() || "",
                description: property.description || "",
                status: property.status || "Available",
                title: property.title || "",
                city: property.city || "",
                state: property.state || "",
                zipCode: property.zipCode || "",
                country: property.country || "",
                propertyType: property.propertyType || ""
            });
        }
    }, [property]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const target = e.target as any;
        const { id, value } = target;
        setFormData(prev => ({ ...prev, [id]: value }));
    };

    const handleStatusChange = (value: string) => {
        setFormData(prev => ({ ...prev, status: value }));
    };

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        const token = (window as any).localStorage.getItem("token");

        try {
            const payload = {
                address: formData.address,
                area: Number(formData.area),
                bedrooms: Number(formData.bedrooms),
                bathrooms: Number(formData.bathrooms),
                floor: formData.floor ? Number(formData.floor) : null,
                monthlyRent: Number(formData.monthlyRent),
                description: formData.description,
                status: formData.status,
                title: formData.title,
                city: formData.city,
                state: formData.state,
                zipCode: formData.zipCode,
                country: formData.country,
                propertyType: formData.propertyType
            };

            const res = await fetch(`/api/properties/${property.id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { "Authorization": `Bearer ${token}` } : {})
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const errorData: any = await res.json();
                throw new Error(errorData.error || "Failed to update property");
            }

            onClose();
        } catch (err: any) {
            setError(err.message);
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="sm:max-w-[500px] max-h-[90vh]">
                <DialogHeader>
                    <DialogTitle>Edit Property</DialogTitle>
                </DialogHeader>
                <form onSubmit={onSubmit} className="grid gap-4 py-4 max-h-[70vh] overflow-y-auto">
                    {error && <div className="text-red-500 text-sm">{error}</div>}

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="title" className="text-right">
                            Title
                        </Label>
                        <Input id="title" value={formData.title} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="address" className="text-right">
                            Address
                        </Label>
                        <Input id="address" value={formData.address} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="city" className="text-right">
                            City
                        </Label>
                        <Input id="city" value={formData.city} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="state" className="text-right">
                            State
                        </Label>
                        <Input id="state" value={formData.state} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="zipCode" className="text-right">
                            Zip Code
                        </Label>
                        <Input id="zipCode" value={formData.zipCode} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="country" className="text-right">
                            Country
                        </Label>
                        <Input id="country" value={formData.country} onChange={handleChange} className="col-span-3" required />
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="propertyType" className="text-right">
                            Type
                        </Label>
                        <div className="col-span-3">
                            <Select value={formData.propertyType} onValueChange={(value) => setFormData(prev => ({ ...prev, propertyType: value }))}>
                                <SelectTrigger>
                                    <SelectValue placeholder="Select type" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="Apartment">Apartment</SelectItem>
                                    <SelectItem value="House">House</SelectItem>
                                    <SelectItem value="Condo">Condo</SelectItem>
                                    <SelectItem value="Studio">Studio</SelectItem>
                                    <SelectItem value="Townhouse">Townhouse</SelectItem>
                                </SelectContent>
                            </Select>
                        </div>
                    </div>

                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label htmlFor="status" className="text-right">
                            Status
                        </Label>
                        <div className="col-span-3">
                            <Select value={formData.status} onValueChange={handleStatusChange}>
                                <SelectTrigger>
                                    <SelectValue placeholder="Select status" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="Available">Available</SelectItem>
                                    <SelectItem value="Rented">Rented</SelectItem>
                                    <SelectItem value="Reserved">Reserved</SelectItem>
                                    <SelectItem value="Maintenance">Maintenance</SelectItem>
                                </SelectContent>
                            </Select>
                        </div>
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
                        <Button type="submit">Save Changes</Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}
