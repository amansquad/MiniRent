"use client";

import { useEffect, useState, use } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ArrowLeft, Home, BedDouble, Layers, Ruler, Calendar, User, Phone, Mail, FileText } from "lucide-react";
import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useToast } from "@/hooks/use-toast";
import { getUser } from "@/lib/auth";
import AddInquiryModal from "@/components/AddInquiryModal";
import { AddRentalModal } from "@/components/AddRentalModal";

interface RentalHistory {
    id: number;
    tenantName: string;
    startDate: string;
    endDate: string | null;
    monthlyRent: number;
    deposit: number;
    status: string;
    createdBy: string;
}

interface Property {
    id: number;
    address: string;
    area: number;
    bedrooms: number;
    floor: number | null;
    monthlyRent: number;
    status: string;
    description: string | null;
    imageUrl: string | null;
    createdBy: string;
    createdById: number;
    recentRentals: RentalHistory[];
}

export default function PropertyDetailPage() {
    const params = useParams();
    const router = useRouter();
    const { toast } = useToast();
    const [property, setProperty] = useState<Property | null>(null);
    const [loading, setLoading] = useState(true);
    const [currentUserId, setCurrentUserId] = useState<number | null>(null);
    const [currentUserRole, setCurrentUserRole] = useState<string | null>(null);
    const [showInquiryModal, setShowInquiryModal] = useState(false);
    const [showRentalModal, setShowRentalModal] = useState(false);

    useEffect(() => {
        const user = getUser();
        if (user) {
            setCurrentUserId(user.id);
            setCurrentUserRole(user.role || user.Role);
        }
        fetchProperty();
    }, [params.id]);

    const fetchProperty = async () => {
        setLoading(true);
        try {
            const token = typeof (window as any) !== "undefined" ? ((window as any).localStorage.getItem)("token") : null;
            const res = await fetch(`/api/properties/${params.id}`, {
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });

            if (!res.ok) {
                if (res.status === 404) {
                    toast({ title: "Error", description: "Property not found", variant: "destructive" });
                    router.push("/properties");
                    return;
                }
                throw new Error("Failed to fetch property");
            }

            const data = await res.json();
            setProperty(data);
        } catch (error) {
            console.error("Error:", error);
            toast({ title: "Error", description: "Failed to load property details", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <div className="p-8 text-center text-muted-foreground">Loading details...</div>;
    if (!property) return null;

    if (!property) return null;

    const isOwner = currentUserId === property.createdById || currentUserRole === "Admin";

    return (
        <div className="p-8 max-w-5xl mx-auto">
            <Button variant="ghost" className="mb-6 hover:bg-secondary/50" asChild>
                <Link href="/properties">
                    <ArrowLeft className="w-4 h-4 mr-2" />
                    Back to Properties
                </Link>
            </Button>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Left Column: Property Details */}
                <div className="lg:col-span-2 space-y-6">
                    <Card className="p-6">
                        <div className="flex justify-between items-start mb-4">
                            <div>
                                <h1 className="text-3xl font-bold">{property.address}</h1>
                                <p className="text-xl text-primary font-semibold mt-1">${property.monthlyRent}/month</p>
                            </div>
                            <Badge variant={property.status.toLowerCase() === "available" ? "default" : "secondary"} className="text-sm px-3 py-1">
                                {property.status}
                            </Badge>
                        </div>

                        <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-8">
                            <div className="flex flex-col items-center p-3 bg-muted rounded-lg">
                                <BedDouble className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium">{property.bedrooms} Bedrooms</span>
                            </div>
                            <div className="flex flex-col items-center p-3 bg-muted rounded-lg">
                                <Ruler className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium">{property.area} sqm</span>
                            </div>
                            <div className="flex flex-col items-center p-3 bg-muted rounded-lg">
                                <Layers className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium">Floor {property.floor ?? "G"}</span>
                            </div>
                            <div className="flex flex-col items-center p-3 bg-muted rounded-lg">
                                <User className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium truncate w-full text-center">{property.createdBy}</span>
                            </div>
                        </div>

                        <div className="mt-8">
                            <h2 className="text-lg font-semibold mb-3">Description</h2>
                            <p className="text-muted-foreground leading-relaxed">
                                {property.description || "No description available for this property."}
                            </p>
                        </div>
                    </Card>

                    {/* Rental History Section */}
                    <Card className="p-6">
                        <h2 className="text-xl font-bold mb-6 flex items-center">
                            <FileText className="w-5 h-5 mr-2 text-primary" />
                            Rental History (Recent)
                        </h2>
                        {property.recentRentals.length === 0 ? (
                            <p className="text-muted-foreground text-center py-4 bg-muted/30 rounded-lg">No rental history available.</p>
                        ) : (
                            <div className="space-y-4">
                                {property.recentRentals.map((rental) => (
                                    <div key={rental.id} className="flex justify-between items-center p-4 border rounded-lg hover:border-primary/50 transition-colors">
                                        <div className="space-y-1">
                                            <p className="font-semibold text-lg">{rental.tenantName}</p>
                                            <div className="flex gap-4 text-sm text-muted-foreground">
                                                <span className="flex items-center">
                                                    <Calendar className="w-3.5 h-3.5 mr-1" />
                                                    {new Date(rental.startDate).toLocaleDateString()}
                                                </span>
                                                {rental.endDate && (
                                                    <span className="flex items-center">
                                                        - {new Date(rental.endDate).toLocaleDateString()}
                                                    </span>
                                                )}
                                            </div>
                                        </div>
                                        <div className="text-right">
                                            <p className="font-medium text-primary">${rental.monthlyRent}</p>
                                            <Badge variant="outline" className="mt-1">{rental.status}</Badge>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </Card>
                </div>

                {/* Right Column: Actions */}
                <div className="space-y-6">
                    <Card className="p-6 sticky top-8">
                        <h2 className="text-xl font-bold mb-4">Actions</h2>
                        <div className="space-y-3">
                            {isOwner ? (
                                <Button className="w-full" variant="outline" onClick={() => toast({ title: "Notice", description: "Use the edit button on the main page to modify property details." })}>
                                    Manage Property
                                </Button>
                            ) : (
                                <>
                                    <Button className="w-full text-lg py-6" onClick={() => setShowInquiryModal(true)}>
                                        Send Inquiry
                                    </Button>
                                    {property.status.toLowerCase() === "available" && (
                                        <Button className="w-full text-lg py-6" variant="outline" onClick={() => setShowRentalModal(true)}>
                                            Request to Rent
                                        </Button>
                                    )}
                                </>
                            )}
                        </div>

                        <div className="mt-8 pt-8 border-t space-y-4">
                            <h3 className="font-semibold text-sm uppercase tracking-wider text-muted-foreground">Contact Property Owner</h3>
                            <div className="flex items-center text-sm">
                                <User className="w-4 h-4 mr-3 text-primary" />
                                <span>{property.createdBy}</span>
                            </div>
                            <div className="flex items-center text-sm text-muted-foreground">
                                <Mail className="w-4 h-4 mr-3" />
                                <span>Contact via Inquiry</span>
                            </div>
                        </div>
                    </Card>
                </div>
            </div>

            {showInquiryModal && (
                <AddInquiryModal
                    isOpen={showInquiryModal}
                    onClose={() => setShowInquiryModal(false)}
                    propertyId={property.id}
                    propertyAddress={property.address}
                />
            )}

            {showRentalModal && (
                <AddRentalModal
                    isOpen={showRentalModal}
                    onClose={() => setShowRentalModal(false)}
                    property={property}
                />
            )}
        </div>
    );
}
