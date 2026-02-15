"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Plus, ChevronLeft, ChevronRight, Search, SlidersHorizontal, Eye } from "lucide-react";
import { cn } from "@/lib/utils";
import Link from "next/link";
import { AddPropertyModal } from "@/components/AddPropertyModal";
import { EditPropertyModal } from "@/components/EditPropertyModal";
import AddInquiryModal from "@/components/AddInquiryModal";
import { AddRentalModal } from "@/components/AddRentalModal";
import { useToast } from "@/hooks/use-toast";
import { getUser } from "@/lib/auth";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

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

    // Pagination & Filtering State
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [filters, setFilters] = useState({
        status: "all",
        minBedrooms: "",
        minRent: "",
        maxRent: "",
        searchAddress: "",
        sortBy: "date",
        sortOrder: "desc"
    });
    const [showFilters, setShowFilters] = useState(false);

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

    const fetchProperties = async (pageToFetch = page) => {
        setLoading(true);
        try {
            const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;

            const queryParams = new URLSearchParams({
                mode: filter,
                page: pageToFetch.toString(),
                pageSize: "9", // 3x3 grid
                ...(filters.status !== "all" ? { status: filters.status } : {}),
                ...(filters.minBedrooms ? { minBedrooms: filters.minBedrooms } : {}),
                ...(filters.minRent ? { minRent: filters.minRent } : {}),
                ...(filters.maxRent ? { maxRent: filters.maxRent } : {}),
                ...(filters.searchAddress ? { searchAddress: filters.searchAddress } : {}),
                ...(filters.sortBy ? { sortBy: filters.sortBy } : {}),
                ...(filters.sortOrder ? { sortOrder: filters.sortOrder } : {}),
            });

            const res = await fetch(`/api/properties?${queryParams}`, {
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });

            if (res.status === 401) {
                if (typeof (window as any) !== "undefined") {
                    (window as any).location.href = "/auth?reason=login-required";
                }
                return;
            }

            const json: any = await res.json();
            const data = Array.isArray(json) ? json : (json.data || []);
            const pagination = json.pagination || { totalPages: 1 };

            setProperties(data);
            setTotalPages(pagination.totalPages);
            setPage(pageToFetch);
        } catch (error) {
            console.error("Failed to fetch properties:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (typeof (window as any) !== "undefined" && !(window as any).confirm("Are you sure you want to delete this property?")) return;

        try {
            const token = typeof (window as any) !== "undefined" ? (window as any).localStorage.getItem("token") : null;
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
                <div className="flex gap-3">
                    <Button variant="outline" size="sm" onClick={() => setShowFilters(!showFilters)} className={cn(showFilters && "bg-secondary")}>
                        <SlidersHorizontal className="w-4 h-4 mr-2" />
                        Filters
                    </Button>
                    <div className="flex bg-muted rounded-lg p-1">
                        <button
                            onClick={() => setFilter("all")}
                            className={`px-4 py-1 rounded-md text-sm font-medium transition-colors ${filter === "all" ? "bg-background shadow-sm" : "hover:bg-background/50"
                                }`}
                        >
                            All
                        </button>
                        <button
                            onClick={() => setFilter("my")}
                            className={`px-4 py-1 rounded-md text-sm font-medium transition-colors ${filter === "my" ? "bg-background shadow-sm" : "hover:bg-background/50"
                                }`}
                        >
                            My
                        </button>
                    </div>
                    <Button onClick={() => setIsAddModalOpen(true)}>
                        <Plus className="w-4 h-4 mr-2" />
                        Add New
                    </Button>
                </div>
            </div>

            {/* Filters UI */}
            {showFilters && (
                <Card className="p-4 mb-6 bg-muted/30 border-dashed">
                    <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                        <div className="space-y-1">
                            <label className="text-xs font-semibold text-muted-foreground uppercase">Search Address</label>
                            <div className="relative">
                                <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                                <Input
                                    placeholder="Enter address..."
                                    className="pl-8 h-9"
                                    value={filters.searchAddress}
                                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setFilters(prev => ({ ...prev, searchAddress: e.target.value }))}
                                />
                            </div>
                        </div>

                        <div className="space-y-1">
                            <label className="text-xs font-semibold text-muted-foreground uppercase">Status</label>
                            <Select value={filters.status} onValueChange={(val) => setFilters(prev => ({ ...prev, status: val }))}>
                                <SelectTrigger className="h-9">
                                    <SelectValue placeholder="All Statuses" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="all">All Statuses</SelectItem>
                                    <SelectItem value="Available">Available</SelectItem>
                                    <SelectItem value="Rented">Rented</SelectItem>
                                    <SelectItem value="Reserved">Reserved</SelectItem>
                                    <SelectItem value="Maintenance">Maintenance</SelectItem>
                                </SelectContent>
                            </Select>
                        </div>

                        <div className="grid grid-cols-2 gap-2">
                            <div className="space-y-1">
                                <label className="text-xs font-semibold text-muted-foreground uppercase">Min Rent</label>
                                <Input
                                    type="number"
                                    placeholder="Any"
                                    className="h-9"
                                    value={filters.minRent}
                                    onChange={(e: any) => setFilters(prev => ({ ...prev, minRent: e.target.value }))}
                                />
                            </div>
                            <div className="space-y-1">
                                <label className="text-xs font-semibold text-muted-foreground uppercase">Max Rent</label>
                                <Input
                                    type="number"
                                    placeholder="Any"
                                    className="h-9"
                                    value={filters.maxRent}
                                    onChange={(e: any) => setFilters(prev => ({ ...prev, maxRent: e.target.value }))}
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-2">
                            <div className="space-y-1">
                                <label className="text-xs font-semibold text-muted-foreground uppercase">Min Bedrooms</label>
                                <Input
                                    type="number"
                                    placeholder="Any"
                                    className="h-9"
                                    value={filters.minBedrooms}
                                    onChange={(e: any) => setFilters(prev => ({ ...prev, minBedrooms: e.target.value }))}
                                />
                            </div>
                            <div className="space-y-1">
                                <label className="text-xs font-semibold text-muted-foreground uppercase">Sort By</label>
                                <Select value={filters.sortBy} onValueChange={(val) => setFilters(prev => ({ ...prev, sortBy: val }))}>
                                    <SelectTrigger className="h-9">
                                        <SelectValue />
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value="date">Newest</SelectItem>
                                        <SelectItem value="rent">Rent Price</SelectItem>
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>

                        <div className="flex items-end gap-2">
                            <Button className="flex-1 h-9" onClick={() => fetchProperties(1)}>
                                Apply Filters
                            </Button>
                            <Button variant="ghost" className="h-9" onClick={() => {
                                setFilters({ status: "all", minBedrooms: "", minRent: "", maxRent: "", searchAddress: "", sortBy: "date", sortOrder: "desc" });
                                // fetch will be triggered by re-render or manual call
                            }}>
                                Reset
                            </Button>
                        </div>
                    </div>
                </Card>
            )}

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
                                    "p-0 overflow-hidden transition-all duration-500 flex flex-col group",
                                    highlightedId === property.id && "border-primary border-2 shadow-lg ring-2 ring-primary/20 scale-[1.02]"
                                )}
                                id={`property-${property.id}`}
                            >
                                <div className="p-4 flex-1">
                                    <div className="flex justify-between items-start">
                                        <div>
                                            <h3 className="font-bold text-lg leading-tight mb-1">{property.address}</h3>
                                            <p className="text-primary font-semibold">
                                                ${property.monthlyRent}/mo
                                            </p>
                                            <div className="flex gap-2 text-xs text-muted-foreground mt-2">
                                                <span>{property.bedrooms} Bed</span>
                                                <span>•</span>
                                                <Badge variant="secondary" className="px-1.5 py-0 h-4 text-[10px] uppercase font-bold tracking-wider">
                                                    {property.status}
                                                </Badge>
                                            </div>
                                        </div>

                                        <div className="flex flex-col gap-2">
                                            {/* Show Edit/Delete if it's "My Properties" OR user owns it */}
                                            {(filter === "my" || (currentUserId && property.createdById === currentUserId)) ? (
                                                <div className="flex gap-1">
                                                    <Button variant="outline" size="icon" className="h-8 w-8" onClick={() => setEditProperty(property)}>
                                                        <Plus className="h-4 w-4 rotate-45" /> {/* Use as edit icon or similar */}
                                                    </Button>
                                                    <Button variant="destructive" size="icon" className="h-8 w-8" onClick={() => handleDelete(property.id)}>
                                                        <Plus className="h-4 w-4 rotate-45" />
                                                    </Button>
                                                </div>
                                            ) : (
                                                /* Action buttons moved to bottom for cleaner look */
                                                null
                                            )}
                                        </div>
                                    </div>
                                </div>

                                <div className="p-3 bg-muted/30 border-t flex gap-2">
                                    <Button variant="secondary" size="sm" className="flex-1 h-8 text-xs" asChild>
                                        <Link href={`/properties/${property.id}`}>
                                            <Eye className="w-3 h-3 mr-1.5" />
                                            View Details
                                        </Link>
                                    </Button>

                                    {!(filter === "my" || (currentUserId && property.createdById === currentUserId)) && (
                                        <>
                                            <Button size="sm" className="flex-1 h-8 text-xs" onClick={() => setInquiryProperty(property)}>
                                                Inquire
                                            </Button>
                                            {property.status.toLowerCase() === "available" && (
                                                <Button size="sm" variant="outline" className="flex-1 h-8 text-xs border-primary text-primary hover:bg-primary/10" onClick={() => setRentProperty(property)}>
                                                    Rent
                                                </Button>
                                            )}
                                        </>
                                    )}
                                </div>
                            </Card>
                        ))
                    )}
                </div>
            )}

            {/* Pagination UI */}
            {totalPages > 1 && (
                <div className="flex justify-center items-center gap-4 mt-10">
                    <Button
                        variant="ghost"
                        size="sm"
                        disabled={page === 1}
                        onClick={() => fetchProperties(page - 1)}
                    >
                        <ChevronLeft className="w-4 h-4 mr-1" />
                        Previous
                    </Button>
                    <span className="text-sm font-medium">
                        Page {page} of {totalPages}
                    </span>
                    <Button
                        variant="ghost"
                        size="sm"
                        disabled={page === totalPages}
                        onClick={() => fetchProperties(page + 1)}
                    >
                        Next
                        <ChevronRight className="w-4 h-4 ml-1" />
                    </Button>
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
