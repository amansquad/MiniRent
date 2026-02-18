"use client"

import { useState, useRef, useEffect } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Upload, Camera, Link as LinkIcon, X, Plus } from "lucide-react";
import { useToast } from "@/hooks/use-toast";

export function AddPropertyModal({ isOpen, onClose }: { isOpen: boolean; onClose: () => void }) {
    const [error, setError] = useState("");
    const [mounted, setMounted] = useState(false);
    const { toast } = useToast();
    const [formData, setFormData] = useState({
        title: "",
        address: "",
        city: "",
        state: "",
        zipCode: "",
        country: "",
        propertyType: "",
        area: "",
        bedrooms: "",
        bathrooms: "",
        floor: "",
        monthlyRent: "",
        description: ""
    });

    const [images, setImages] = useState<Array<{ url: string; caption: string; isPrimary: boolean }>>([]);
    const [currentImage, setCurrentImage] = useState({ url: "", caption: "" });
    const [uploadMethod, setUploadMethod] = useState<"url" | "file" | "camera">("url");
    const [previewUrl, setPreviewUrl] = useState<string | null>(null);
    const [cameraStream, setCameraStream] = useState<MediaStream | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const videoRef = useRef<HTMLVideoElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);

    useEffect(() => {
        setMounted(true);
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { id, value } = e.target;
        setFormData(prev => ({ ...prev, [id]: value }));
    };

    const convertToBase64 = (file: File): Promise<string> => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result as string);
            reader.onerror = error => reject(error);
        });
    };

    const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        if (!file.type.startsWith('image/')) {
            toast({ title: "Error", description: "Please select an image file", variant: "destructive" });
            return;
        }

        try {
            const base64 = await convertToBase64(file);
            setPreviewUrl(base64);
            setCurrentImage(prev => ({ ...prev, url: base64 }));
        } catch (error) {
            toast({ title: "Error", description: "Failed to process image", variant: "destructive" });
        }
    };

    const startCamera = async () => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' } });
            setCameraStream(stream);
            if (videoRef.current) videoRef.current.srcObject = stream;
        } catch (error) {
            toast({ title: "Error", description: "Could not access camera", variant: "destructive" });
        }
    };

    const stopCamera = () => {
        if (cameraStream) {
            cameraStream.getTracks().forEach(track => track.stop());
            setCameraStream(null);
        }
    };

    const capturePhoto = () => {
        if (videoRef.current && canvasRef.current) {
            const video = videoRef.current;
            const canvas = canvasRef.current;
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            const ctx = canvas.getContext('2d');
            if (ctx) {
                ctx.drawImage(video, 0, 0);
                const imageData = canvas.toDataURL('image/jpeg');
                setPreviewUrl(imageData);
                setCurrentImage(prev => ({ ...prev, url: imageData }));
                stopCamera();
            }
        }
    };

    const addImageToList = () => {
        if (!currentImage.url.trim()) {
            toast({ title: "Error", description: "Please provide an image", variant: "destructive" });
            return;
        }

        setImages(prev => [...prev, { ...currentImage, isPrimary: prev.length === 0 }]);
        setCurrentImage({ url: "", caption: "" });
        setPreviewUrl(null);
        stopCamera();
        if (fileInputRef.current) fileInputRef.current.value = "";
    };

    const removeImage = (index: number) => {
        setImages(prev => prev.filter((_, i) => i !== index));
    };

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        if (typeof window === 'undefined') return;

        const token = window.localStorage.getItem("token");

        try {
            const payload = {
                title: formData.title,
                address: formData.address,
                city: formData.city,
                state: formData.state,
                zipCode: formData.zipCode,
                country: formData.country,
                propertyType: formData.propertyType,
                area: Number(formData.area),
                bedrooms: Number(formData.bedrooms),
                bathrooms: Number(formData.bathrooms),
                floor: formData.floor ? Number(formData.floor) : null,
                monthlyRent: Number(formData.monthlyRent),
                description: formData.description
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

            const property = await res.json();

            // Upload images if any
            if (images.length > 0) {
                for (const image of images) {
                    await fetch(`/api/properties/${property.id}/images`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            ...(token ? { "Authorization": `Bearer ${token}` } : {})
                        },
                        body: JSON.stringify(image)
                    });
                }
            }

            toast({ title: "Success", description: "Property created successfully" });

            // Reset form
            setFormData({
                title: "",
                address: "",
                city: "",
                state: "",
                zipCode: "",
                country: "",
                propertyType: "",
                area: "",
                bedrooms: "",
                bathrooms: "",
                floor: "",
                monthlyRent: "",
                description: ""
            });
            setImages([]);
            stopCamera();
            onClose();
        } catch (err: any) {
            setError(err.message);
        }
    };

    if (!mounted) {
        return null;
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => {
            if (!open) stopCamera();
            onClose();
        }}>
            <DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle>Add New Property</DialogTitle>
                </DialogHeader>
                <form onSubmit={onSubmit} className="space-y-4 py-4">
                    {error && <div className="text-red-500 text-sm p-3 bg-red-50 rounded">{error}</div>}

                    <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="title">Title *</Label>
                            <Input id="title" value={formData.title} onChange={handleChange} required />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="propertyType">Type *</Label>
                            <Select value={formData.propertyType} onValueChange={(v) => setFormData(prev => ({ ...prev, propertyType: v }))}>
                                <SelectTrigger>
                                    <SelectValue placeholder="Select" />
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

                    <div className="space-y-2">
                        <Label htmlFor="address">Address *</Label>
                        <Input id="address" value={formData.address} onChange={handleChange} required />
                    </div>

                    <div className="grid grid-cols-3 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="city">City *</Label>
                            <Input id="city" value={formData.city} onChange={handleChange} required />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="state">State *</Label>
                            <Input id="state" value={formData.state} onChange={handleChange} required />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="zipCode">Zip *</Label>
                            <Input id="zipCode" value={formData.zipCode} onChange={handleChange} required />
                        </div>
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="country">Country *</Label>
                        <Input id="country" value={formData.country} onChange={handleChange} required />
                    </div>

                    <div className="grid grid-cols-4 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="area">Area (m²) *</Label>
                            <Input id="area" type="number" value={formData.area} onChange={handleChange} required />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="bedrooms">Beds *</Label>
                            <Input id="bedrooms" type="number" value={formData.bedrooms} onChange={handleChange} required />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="bathrooms">Baths *</Label>
                            <Input id="bathrooms" type="number" step="0.5" value={formData.bathrooms} onChange={handleChange} required />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="floor">Floor</Label>
                            <Input id="floor" type="number" value={formData.floor} onChange={handleChange} />
                        </div>
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="monthlyRent">Monthly Rent ($) *</Label>
                        <Input id="monthlyRent" type="number" value={formData.monthlyRent} onChange={handleChange} required />
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="description">Description</Label>
                        <Textarea id="description" value={formData.description} onChange={handleChange} rows={3} />
                    </div>

                    {/* Image Upload Section */}
                    <div className="space-y-3 p-4 border rounded-lg bg-muted/30">
                        <h3 className="font-semibold text-sm">Property Images (Optional)</h3>

                        <Tabs value={uploadMethod} onValueChange={(v) => {
                            setUploadMethod(v as any);
                            setPreviewUrl(null);
                            setCurrentImage({ url: "", caption: "" });
                            stopCamera();
                        }}>
                            <TabsList className="grid w-full grid-cols-3">
                                <TabsTrigger value="url"><LinkIcon className="w-3 h-3 mr-1" />URL</TabsTrigger>
                                <TabsTrigger value="file"><Upload className="w-3 h-3 mr-1" />Upload</TabsTrigger>
                                <TabsTrigger value="camera"><Camera className="w-3 h-3 mr-1" />Camera</TabsTrigger>
                            </TabsList>

                            <TabsContent value="url" className="space-y-2">
                                <Input
                                    placeholder="https://example.com/image.jpg"
                                    value={currentImage.url}
                                    onChange={(e) => setCurrentImage(prev => ({ ...prev, url: e.target.value }))}
                                />
                            </TabsContent>

                            <TabsContent value="file" className="space-y-2">
                                <Input
                                    ref={fileInputRef}
                                    type="file"
                                    accept="image/*"
                                    onChange={handleFileSelect}
                                    className="cursor-pointer"
                                />
                            </TabsContent>

                            <TabsContent value="camera" className="space-y-2">
                                {!cameraStream && !previewUrl && (
                                    <Button type="button" onClick={startCamera} className="w-full" size="sm">
                                        <Camera className="w-3 h-3 mr-1" />Start Camera
                                    </Button>
                                )}
                                {cameraStream && (
                                    <div className="space-y-2">
                                        <video ref={videoRef} autoPlay playsInline className="w-full rounded border" />
                                        <div className="flex gap-2">
                                            <Button type="button" onClick={capturePhoto} size="sm" className="flex-1">Capture</Button>
                                            <Button type="button" variant="outline" onClick={stopCamera} size="sm">Cancel</Button>
                                        </div>
                                    </div>
                                )}
                                <canvas ref={canvasRef} className="hidden" />
                            </TabsContent>
                        </Tabs>

                        {previewUrl && (
                            <div className="relative">
                                <img src={previewUrl} alt="Preview" className="w-full h-32 object-cover rounded border" />
                            </div>
                        )}

                        <Input
                            placeholder="Image caption (optional)"
                            value={currentImage.caption}
                            onChange={(e) => setCurrentImage(prev => ({ ...prev, caption: e.target.value }))}
                            maxLength={200}
                        />

                        <Button type="button" onClick={addImageToList} disabled={!currentImage.url} size="sm" className="w-full">
                            <Plus className="w-3 h-3 mr-1" />Add Image
                        </Button>

                        {images.length > 0 && (
                            <div className="space-y-2">
                                <p className="text-xs text-muted-foreground">Added Images ({images.length})</p>
                                <div className="grid grid-cols-3 gap-2">
                                    {images.map((img, idx) => (
                                        <div key={idx} className="relative group">
                                            <img src={img.url} alt={img.caption || `Image ${idx + 1}`} className="w-full h-20 object-cover rounded border" />
                                            <Button
                                                type="button"
                                                variant="destructive"
                                                size="icon"
                                                className="absolute top-1 right-1 h-6 w-6 opacity-0 group-hover:opacity-100"
                                                onClick={() => removeImage(idx)}
                                            >
                                                <X className="w-3 h-3" />
                                            </Button>
                                            {img.isPrimary && (
                                                <span className="absolute bottom-1 left-1 bg-primary text-primary-foreground text-[10px] px-1 rounded">Primary</span>
                                            )}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>

                    <div className="flex justify-end gap-2">
                        <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
                        <Button type="submit">Create Property</Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}
