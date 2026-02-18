"use client";

import { useEffect, useState, use } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ArrowLeft, Home, BedDouble, Bath, Layers, Ruler, Calendar, User, Phone, Mail, FileText, Star, Image as ImageIcon, X, Upload } from "lucide-react";
import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useToast } from "@/hooks/use-toast";
import { getUser } from "@/lib/auth";
import AddInquiryModal from "@/components/AddInquiryModal";
import { AddRentalModal } from "@/components/AddRentalModal";
import { AddReviewModal } from "@/components/AddReviewModal";
import { PropertyImageManager } from "@/components/PropertyImageManager";

interface RentalHistory {
    id: string;
    tenantName: string;
    startDate: string;
    endDate: string | null;
    monthlyRent: number;
    deposit: number;
    status: string;
    createdBy: string;
}

interface PropertyImage {
    id: string;
    url: string;
    caption: string | null;
    isPrimary: boolean;
    createdAt: string;
}

interface Review {
    id: string;
    rating: number;
    comment: string | null;
    createdAt: string;
    reviewerName: string;
}

interface Property {
    id: string;
    address: string;
    city: string;
    state: string;
    country: string;
    area: number;
    bedrooms: number;
    bathrooms: number;
    floor: number | null;
    monthlyRent: number;
    status: string;
    propertyType: string;
    description: string | null;
    imageUrl: string | null;
    createdBy: string;
    createdById: string;
    recentRentals: RentalHistory[];
    images: PropertyImage[];
    reviews: Review[];
}

export default function PropertyDetailPage() {
    const params = useParams();
    const router = useRouter();
    const { toast } = useToast();
    const [property, setProperty] = useState<Property | null>(null);
    const [loading, setLoading] = useState(true);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const [currentUserRole, setCurrentUserRole] = useState<string | null>(null);
    const [showInquiryModal, setShowInquiryModal] = useState(false);
    const [showRentalModal, setShowRentalModal] = useState(false);
    const [showReviewModal, setShowReviewModal] = useState(false);
    const [showImageManager, setShowImageManager] = useState(false);
    const [selectedImage, setSelectedImage] = useState<string | null>(null);
    const [hasRented, setHasRented] = useState(false);

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

            // Check if user has rented this property
            if (currentUserId && data.recentRentals) {
                const userRental = data.recentRentals.find((r: any) =>
                    r.tenantId === currentUserId && (r.status === "Ended" || r.status === "Active")
                );
                setHasRented(!!userRental);
            }
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
                    {/* Image Gallery */}
                    {property.images && property.images.length > 0 && (
                        <Card className="p-0 overflow-hidden">
                            <div className="relative aspect-video bg-muted">
                                <img
                                    src={selectedImage || property.images.find(i => i.isPrimary)?.url || property.images[0]?.url}
                                    alt={property.address}
                                    className="w-full h-full object-cover cursor-pointer"
                                    onClick={() => setSelectedImage(selectedImage || property.images.find(i => i.isPrimary)?.url || property.images[0]?.url)}
                                />
                                {isOwner && (
                                    <Button
                                        size="sm"
                                        variant="secondary"
                                        className="absolute top-4 right-4"
                                        onClick={() => setShowImageManager(true)}
                                    >
                                        <Upload className="w-4 h-4 mr-2" />
                                        Manage Images
                                    </Button>
                                )}
                            </div>
                            {property.images.length > 1 && (
                                <div className="flex gap-2 p-4 overflow-x-auto">
                                    {property.images.map((img) => (
                                        <div
                                            key={img.id}
                                            className={`relative flex-shrink-0 w-20 h-20 rounded-lg overflow-hidden cursor-pointer border-2 ${selectedImage === img.url ? 'border-primary' : 'border-transparent'
                                                }`}
                                            onClick={() => setSelectedImage(img.url)}
                                        >
                                            <img src={img.url} alt={img.caption || ''} className="w-full h-full object-cover" />
                                        </div>
                                    ))}
                                </div>
                            )}
                        </Card>
                    )}

                    {isOwner && (!property.images || property.images.length === 0) && (
                        <Card className="p-6 text-center">
                            <ImageIcon className="w-12 h-12 mx-auto mb-3 text-muted-foreground" />
                            <p className="text-muted-foreground mb-4">No images added yet</p>
                            <Button onClick={() => setShowImageManager(true)}>
                                <Upload className="w-4 h-4 mr-2" />
                                Add Images
                            </Button>
                        </Card>
                    )}

                    <Card className="p-6">
                        <div className="flex justify-between items-start mb-4">
                            <div className="flex-1">
                                <div className="flex items-center gap-3 mb-2">
                                    <h1 className="text-3xl font-bold">{property.address}</h1>
                                    <Badge variant="outline" className="text-sm">
                                        <Home className="w-3 h-3 mr-1" />
                                        {property.propertyType}
                                    </Badge>
                                </div>
                                <p className="text-muted-foreground mb-2">
                                    {property.city}, {property.state}, {property.country}
                                </p>
                                <p className="text-xl text-primary font-semibold">${property.monthlyRent}/month</p>
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
                                <Bath className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium">{property.bathrooms} Bathrooms</span>
                            </div>
                            <div className="flex flex-col items-center p-3 bg-muted rounded-lg">
                                <Ruler className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium">{property.area} sqm</span>
                            </div>
                            <div className="flex flex-col items-center p-3 bg-muted rounded-lg">
                                <Layers className="w-5 h-5 mb-2 text-primary" />
                                <span className="text-sm font-medium">Floor {property.floor ?? "G"}</span>
                            </div>
                        </div>

                        <div className="mt-8">
                            <h2 className="text-lg font-semibold mb-3">Description</h2>
                            <p className="text-muted-foreground leading-relaxed">
                                {property.description || "No description available for this property."}
                            </p>
                        </div>
                    </Card>

                    {/* Reviews Section */}
                    <Card className="p-6">
                        <div className="flex justify-between items-center mb-6">
                            <h2 className="text-xl font-bold flex items-center">
                                <Star className="w-5 h-5 mr-2 text-primary" />
                                Reviews ({property.reviews?.length || 0})
                            </h2>
                            {hasRented && !isOwner && (
                                <Button size="sm" onClick={() => setShowReviewModal(true)}>
                                    Write Review
                                </Button>
                            )}
                        </div>
                        {!property.reviews || property.reviews.length === 0 ? (
                            <p className="text-muted-foreground text-center py-4 bg-muted/30 rounded-lg">No reviews yet.</p>
                        ) : (
                            <div className="space-y-4">
                                {property.reviews.map((review) => (
                                    <div key={review.id} className="border-b pb-4 last:border-0">
                                        <div className="flex justify-between items-start mb-2">
                                            <div>
                                                <p className="font-semibold">{review.reviewerName}</p>
                                                <div className="flex items-center gap-1 mt-1">
                                                    {[...Array(5)].map((_, i) => (
                                                        <Star
                                                            key={i}
                                                            className={`w-4 h-4 ${i < review.rating ? 'fill-yellow-400 text-yellow-400' : 'text-gray-300'}`}
                                                        />
                                                    ))}
                                                </div>
                                            </div>
                                            <span className="text-xs text-muted-foreground">
                                                {new Date(review.createdAt).toLocaleDateString()}
                                            </span>
                                        </div>
                                        {review.comment && (
                                            <p className="text-sm text-muted-foreground mt-2">{review.comment}</p>
                                        )}
                                    </div>
                                ))}
                            </div>
                        )}
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

            {showReviewModal && (
                <AddReviewModal
                    isOpen={showReviewModal}
                    onClose={() => {
                        setShowReviewModal(false);
                        fetchProperty();
                    }}
                    propertyId={property.id}
                    propertyAddress={property.address}
                />
            )}

            {showImageManager && (
                <PropertyImageManager
                    isOpen={showImageManager}
                    onClose={() => {
                        setShowImageManager(false);
                        fetchProperty();
                    }}
                    propertyId={property.id}
                    images={property.images || []}
                />
            )}
        </div>
    );
}
