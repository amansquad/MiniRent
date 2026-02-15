"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Plus } from "lucide-react";
import { cn } from "@/lib/utils";
import { AddPropertyModal } from "@/components/AddPropertyModal";
import { EditPropertyModal } from "@/components/EditPropertyModal";
import AddInquiryModal from "@/components/AddInquiryModal";
import { AddRentalModal } from "@/components/AddRentalModal";
import { useToast } from "@/hooks/use-toast";
import { getUser } from "@/lib/auth";

type Property = {
    id: number;
    address: string;
    monthlyRent: number;
    bedrooms: number;
    status: string;
    createdById?: number;
    createdBy?: string;
};

export default function PropertiesPage() {
    const [properties, setProperties] = useState<Property[]>([]);
    const [filter, setFilter] = useState<"all" | "my">("all");
    const [loading, setLoading] = useState(true);
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);
    const [editProperty, setEditProperty] = useState<Property | null>(null);
    const [inquiryProperty, setInquiryProperty] = useState<Property | null>(null);
    const [rentProperty, setRentProperty] = useState<Property | null>(null);
    const [currentUserId, setCurrentUserId] = useState<number | null>(null);
    const [highlightedId, setHighlightedId] = useState<number | null>(null);
    const { toast } = useToast();

    useEffect(() => {
        // Get current user ID
        const user = getUser();
        if (user) {
            setCurrentUserId(user.id);
        }
        fetchProperties();
    }, [filter]);

    // Handle "id" query parameter for highlighting/opening a specific property
    useEffect(() => {
        if (typeof (window as any) === "undefined" || properties.length === 0) return;

        const params = new URLSearchParams((window as any).location.search);
        const idParam = params.get("id");
        if (idParam) {
            const id = parseInt(idParam);
            const property = properties.find(p => p.id === id);

            if (property) {
                setHighlightedId(id);

                // If we found the property, let's also "open" it
                if (currentUserId && property.createdById === currentUserId) {
                    setEditProperty(property);
                } else if (property.status.toLowerCase() === "available") {
                    setInquiryProperty(property);
                }

                // Clear the param
                (window as any).history.replaceState({}, "", "/properties");
            }
        }
    }, [properties, currentUserId]);

    // Handle "rent" query parameter for auto-opening modal
    useEffect(() => {
        if (typeof (window as any) === "undefined" || properties.length === 0) return;

        const params = new URLSearchParams((window as any).location.search);
        const rentId = params.get("rent");
        if (rentId) {
            const property = properties.find(p => p.id === parseInt(rentId));
            if (property && property.status.toLowerCase() === "available") {
                setRentProperty(property);
                // Clear the param to avoid re-opening on manual refresh/filter change
                (window as any).history.replaceState({}, "", "/properties");
            }
        }
    }, [properties]);

    const fetchProperties = async () => {
        setLoading(true);
        try {
            const token = typeof (window as any) !== "undefined" ? ((window as any).localStorage.getItem)("token") : null;
            const res = await fetch(`/api/properties?mode=${filter}`, {
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });

            if (res.status === 401) {
                if (typeof (window as any) !== "undefined") {
                    (window as any).location.href = "/auth?reason=login-required";
                }
                return;
            }

            const json: any = await res.json();
            // Handle both array and paginated object formats
            const data = Array.isArray(json) ? json : (json.data || []);
            setProperties(data);
        } catch (error) {
            console.error("Failed to fetch properties:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (typeof (window as any) !== "undefined" && !(window as any).confirm("Are you sure you want to delete this property?")) return;

        try {
            const token = ((window as any).localStorage.getItem)("token");
            const res = await fetch(`/api/properties/${id}`, {
                method: "DELETE",
                headers: token ? { "Authorization": `Bearer ${token}` } : {}
            });

            if (res.ok) {
                toast({
                    title: "Success",
                    description: "Property deleted successfully",
                });
                fetchProperties();
            } else {
                const data: any = await res.json();
                toast({
                    title: "Error",
                    description: data.error || "Failed to delete property",
                    variant: "destructive",
                });
            }
        } catch (error) {
            console.error("Failed to delete property:", error);
            toast({
                title: "Error",
                description: "An error occurred",
                variant: "destructive",
            });
        }
    };

    return (
        <div className="p-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold">Properties</h1>
                <div className="flex gap-4">
                    <div className="flex bg-muted rounded-lg p-1">
                        <button
                            onClick={() => setFilter("all")}
                            className={`px-4 py-1 rounded-md text-sm font-medium transition-colors ${filter === "all" ? "bg-background shadow-sm" : "hover:bg-background/50"
                                }`}
                        >
                            All Properties
                        </button>
                        <button
                            onClick={() => setFilter("my")}
                            className={`px-4 py-1 rounded-md text-sm font-medium transition-colors ${filter === "my" ? "bg-background shadow-sm" : "hover:bg-background/50"
                                }`}
                        >
                            My Properties
                        </button>
                    </div>
                    <Button onClick={() => setIsAddModalOpen(true)}>
                        <Plus className="w-4 h-4 mr-2" />
                        Add Property
                    </Button>
                </div>
            </div>

            {loading ? (
                <div>Loading...</div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {properties.length === 0 ? (
                        <div className="col-span-full text-center text-muted-foreground p-8">
                            No properties found.
                        </div>
                    ) : (
                        properties.map((property) => (
                            <Card
                                key={property.id}
                                className={cn(
                                    "p-4 transition-all duration-500",
                                    highlightedId === property.id && "border-primary border-2 shadow-lg ring-2 ring-primary/20 scale-[1.02]"
                                )}
                                id={`property-${property.id}`}
                            >
                                <div className="flex justify-between items-start">
                                    <div>
                                        <h3 className="font-semibold text-lg">{property.address}</h3>
                                        <p className="text-muted-foreground">
                                            ${property.monthlyRent}/month • {property.bedrooms} Bed
                                        </p>
                                        <div className="mt-2 text-sm px-2 py-1 bg-secondary rounded-full inline-block">
                                            {property.status}
                                        </div>
                                    </div>

                                    <div className="flex flex-col gap-2">
                                        {/* Show Edit/Delete if it's "My Properties" OR user owns it */}
                                        {(filter === "my" || (currentUserId && property.createdById === currentUserId)) ? (
                                            <>
                                                <Button variant="outline" size="sm" onClick={() => setEditProperty(property)}>
                                                    Edit
                                                </Button>
                                                <Button variant="destructive" size="sm" onClick={() => handleDelete(property.id)}>
                                                    Delete
                                                </Button>
                                            </>
                                        ) : (
                                            /* Show Inquire/Rent for properties we don't own */
                                            <div className="flex flex-col gap-2">
                                                <Button size="sm" onClick={() => setInquiryProperty(property)}>
                                                    Inquire
                                                </Button>
                                                {property.status.toLowerCase() === "available" && (
                                                    <Button size="sm" variant="outline" className="border-primary text-primary hover:bg-primary/10" onClick={() => setRentProperty(property)}>
                                                        Rent
                                                    </Button>
                                                )}
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </Card>
                        ))
                    )}
                </div>
            )}

            <AddPropertyModal
                isOpen={isAddModalOpen}
                onClose={() => {
                    setIsAddModalOpen(false);
                    fetchProperties();
                }}
            />

            {editProperty && (
                <EditPropertyModal
                    isOpen={!!editProperty}
                    onClose={() => {
                        setEditProperty(null);
                        fetchProperties();
                    }}
                    property={editProperty}
                />
            )}

            {inquiryProperty && (
                <AddInquiryModal
                    isOpen={!!inquiryProperty}
                    onClose={() => setInquiryProperty(null)}
                    propertyId={inquiryProperty.id}
                    propertyAddress={inquiryProperty.address}
                />
            )}

            {rentProperty && (
                <AddRentalModal
                    isOpen={!!rentProperty}
                    onClose={() => setRentProperty(null)}
                    property={rentProperty}
                />
            )}
        </div>
    );
}
