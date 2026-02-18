"use client";

import { useState, useRef } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Trash2, Star, Upload, Camera, Link as LinkIcon, X } from "lucide-react";
import { useToast } from "@/hooks/use-toast";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

interface PropertyImage {
    id: string;
    url: string;
    caption: string | null;
    isPrimary: boolean;
    createdAt: string;
}

interface PropertyImageManagerProps {
    isOpen: boolean;
    onClose: () => void;
    propertyId: string;
    images: PropertyImage[];
}

export function PropertyImageManager({ isOpen, onClose, propertyId, images }: PropertyImageManagerProps) {
    const [newImageUrl, setNewImageUrl] = useState("");
    const [newImageCaption, setNewImageCaption] = useState("");
    const [loading, setLoading] = useState(false);
    const [uploadMethod, setUploadMethod] = useState<"url" | "file" | "camera">("url");
    const [previewUrl, setPreviewUrl] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const videoRef = useRef<HTMLVideoElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const [cameraStream, setCameraStream] = useState<MediaStream | null>(null);
    const { toast } = useToast();

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
            toast({
                title: "Error",
                description: "Please select an image file",
                variant: "destructive",
            });
            return;
        }

        try {
            const base64 = await convertToBase64(file);
            setPreviewUrl(base64);
            setNewImageUrl(base64);
        } catch (error) {
            toast({
                title: "Error",
                description: "Failed to process image",
                variant: "destructive",
            });
        }
    };

    const startCamera = async () => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                video: { facingMode: 'environment' }
            });
            setCameraStream(stream);
            if (videoRef.current) {
                videoRef.current.srcObject = stream;
            }
        } catch (error) {
            toast({
                title: "Error",
                description: "Could not access camera",
                variant: "destructive",
            });
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
                setNewImageUrl(imageData);
                stopCamera();
            }
        }
    };

    const handleAddImage = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!newImageUrl.trim()) {
            toast({
                title: "Error",
                description: "Please provide an image",
                variant: "destructive",
            });
            return;
        }

        setLoading(true);
        try {
            const token = localStorage.getItem("token");
            const res = await fetch(`/api/properties/${propertyId}/images`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { Authorization: `Bearer ${token}` } : {}),
                },
                body: JSON.stringify({
                    url: newImageUrl.trim(),
                    caption: newImageCaption.trim() || null,
                    isPrimary: images.length === 0,
                }),
            });

            if (!res.ok) {
                const data: any = await res.json();
                throw new Error(data.message || "Failed to add image");
            }

            toast({
                title: "Success",
                description: "Image added successfully",
            });

            setNewImageUrl("");
            setNewImageCaption("");
            setPreviewUrl(null);
            stopCamera();
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

    const handleDeleteImage = async (imageId: string) => {
        if (typeof window !== 'undefined') {
            const confirmed = window.confirm("Are you sure you want to delete this image?");
            if (!confirmed) return;
        }

        try {
            const token = localStorage.getItem("token");
            const res = await fetch(`/api/properties/${propertyId}/images/${imageId}`, {
                method: "DELETE",
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });

            if (!res.ok) {
                throw new Error("Failed to delete image");
            }

            toast({
                title: "Success",
                description: "Image deleted successfully",
            });

            onClose();
        } catch (error: any) {
            toast({
                title: "Error",
                description: error.message,
                variant: "destructive",
            });
        }
    };

    const handleSetPrimary = async (imageId: string) => {
        try {
            const token = localStorage.getItem("token");
            const res = await fetch(`/api/properties/${propertyId}/images/${imageId}/primary`, {
                method: "PATCH",
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });

            if (!res.ok) {
                throw new Error("Failed to set primary image");
            }

            toast({
                title: "Success",
                description: "Primary image updated",
            });

            onClose();
        } catch (error: any) {
            toast({
                title: "Error",
                description: error.message,
                variant: "destructive",
            });
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={(open) => {
            if (!open) {
                stopCamera();
                setPreviewUrl(null);
            }
            onClose();
        }}>
            <DialogContent className="sm:max-w-[700px] max-h-[85vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle>Manage Property Images</DialogTitle>
                </DialogHeader>

                <div className="space-y-6 py-4">
                    {/* Add New Image Form */}
                    <form onSubmit={handleAddImage} className="space-y-4 p-4 border rounded-lg bg-muted/30">
                        <h3 className="font-semibold">Add New Image</h3>

                        <Tabs value={uploadMethod} onValueChange={(v) => {
                            setUploadMethod(v as any);
                            setPreviewUrl(null);
                            setNewImageUrl("");
                            stopCamera();
                        }}>
                            <TabsList className="grid w-full grid-cols-3">
                                <TabsTrigger value="url">
                                    <LinkIcon className="w-4 h-4 mr-2" />
                                    URL
                                </TabsTrigger>
                                <TabsTrigger value="file">
                                    <Upload className="w-4 h-4 mr-2" />
                                    Upload
                                </TabsTrigger>
                                <TabsTrigger value="camera">
                                    <Camera className="w-4 h-4 mr-2" />
                                    Camera
                                </TabsTrigger>
                            </TabsList>

                            <TabsContent value="url" className="space-y-3">
                                <div className="space-y-2">
                                    <Label htmlFor="imageUrl">Image URL</Label>
                                    <Input
                                        id="imageUrl"
                                        placeholder="https://example.com/image.jpg"
                                        value={newImageUrl}
                                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => setNewImageUrl(e.target.value)}
                                        disabled={loading}
                                    />
                                </div>
                            </TabsContent>

                            <TabsContent value="file" className="space-y-3">
                                <div className="space-y-2">
                                    <Label>Select Image File</Label>
                                    <div className="flex gap-2">
                                        <Input
                                            ref={fileInputRef}
                                            type="file"
                                            accept="image/*"
                                            onChange={handleFileSelect}
                                            disabled={loading}
                                            className="cursor-pointer"
                                        />
                                        {previewUrl && (
                                            <Button
                                                type="button"
                                                variant="ghost"
                                                size="icon"
                                                onClick={() => {
                                                    setPreviewUrl(null);
                                                    setNewImageUrl("");
                                                    if (fileInputRef.current) fileInputRef.current.value = "";
                                                }}
                                            >
                                                <X className="w-4 h-4" />
                                            </Button>
                                        )}
                                    </div>
                                </div>
                            </TabsContent>

                            <TabsContent value="camera" className="space-y-3">
                                <div className="space-y-2">
                                    <Label>Capture Photo</Label>
                                    {!cameraStream && !previewUrl && (
                                        <Button
                                            type="button"
                                            onClick={startCamera}
                                            className="w-full"
                                        >
                                            <Camera className="w-4 h-4 mr-2" />
                                            Start Camera
                                        </Button>
                                    )}
                                    {cameraStream && (
                                        <div className="space-y-2">
                                            <video
                                                ref={videoRef}
                                                autoPlay
                                                playsInline
                                                className="w-full rounded-lg border"
                                            />
                                            <div className="flex gap-2">
                                                <Button
                                                    type="button"
                                                    onClick={capturePhoto}
                                                    className="flex-1"
                                                >
                                                    <Camera className="w-4 h-4 mr-2" />
                                                    Capture
                                                </Button>
                                                <Button
                                                    type="button"
                                                    variant="outline"
                                                    onClick={stopCamera}
                                                >
                                                    Cancel
                                                </Button>
                                            </div>
                                        </div>
                                    )}
                                    <canvas ref={canvasRef} className="hidden" />
                                </div>
                            </TabsContent>
                        </Tabs>

                        {/* Preview */}
                        {previewUrl && (
                            <div className="space-y-2">
                                <Label>Preview</Label>
                                <div className="relative">
                                    <img
                                        src={previewUrl}
                                        alt="Preview"
                                        className="w-full h-48 object-cover rounded-lg border"
                                    />
                                </div>
                            </div>
                        )}

                        <div className="space-y-2">
                            <Label htmlFor="imageCaption">Caption (Optional)</Label>
                            <Input
                                id="imageCaption"
                                placeholder="Living room view"
                                value={newImageCaption}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) => setNewImageCaption(e.target.value)}
                                disabled={loading}
                                maxLength={200}
                            />
                        </div>

                        <Button type="submit" disabled={loading || !newImageUrl} className="w-full">
                            <Upload className="w-4 h-4 mr-2" />
                            {loading ? "Adding..." : "Add Image"}
                        </Button>
                    </form>

                    {/* Existing Images */}
                    <div className="space-y-3">
                        <h3 className="font-semibold">Current Images ({images.length})</h3>
                        {images.length === 0 ? (
                            <p className="text-sm text-muted-foreground text-center py-8">No images added yet</p>
                        ) : (
                            <div className="grid grid-cols-2 gap-3">
                                {images.map((image) => (
                                    <div key={image.id} className="relative group border rounded-lg overflow-hidden">
                                        <img
                                            src={image.url}
                                            alt={image.caption || "Property image"}
                                            className="w-full h-32 object-cover"
                                        />
                                        <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2">
                                            <Button
                                                size="sm"
                                                variant={image.isPrimary ? "default" : "secondary"}
                                                onClick={() => handleSetPrimary(image.id)}
                                                disabled={image.isPrimary}
                                            >
                                                <Star className={`w-4 h-4 ${image.isPrimary ? 'fill-current' : ''}`} />
                                            </Button>
                                            <Button
                                                size="sm"
                                                variant="destructive"
                                                onClick={() => handleDeleteImage(image.id)}
                                            >
                                                <Trash2 className="w-4 h-4" />
                                            </Button>
                                        </div>
                                        {image.isPrimary && (
                                            <div className="absolute top-2 left-2 bg-primary text-primary-foreground text-xs px-2 py-1 rounded">
                                                Primary
                                            </div>
                                        )}
                                        {image.caption && (
                                            <div className="absolute bottom-0 left-0 right-0 bg-black/70 text-white text-xs p-2 truncate">
                                                {image.caption}
                                            </div>
                                        )}
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    );
}
