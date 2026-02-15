"use client"

import { useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { AlertTriangle } from "lucide-react";

interface DeleteUserModalProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => Promise<void>;
    userName: string;
    userEmail: string;
}

export default function DeleteUserModal({ isOpen, onClose, onConfirm, userName, userEmail }: DeleteUserModalProps) {
    const [deleting, setDeleting] = useState(false);

    const handleConfirm = async () => {
        setDeleting(true);
        try {
            await onConfirm();
            onClose();
        } catch (error) {
            console.error("Delete failed:", error);
        } finally {
            setDeleting(false);
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <div className="flex items-center gap-3">
                        <div className="flex h-10 w-10 items-center justify-center rounded-full bg-red-100">
                            <AlertTriangle className="h-5 w-5 text-red-600" />
                        </div>
                        <DialogTitle>Delete User Account</DialogTitle>
                    </div>
                    <DialogDescription className="pt-3">
                        Are you sure you want to delete this user account? This action will deactivate the account and the user will no longer be able to access the system.
                    </DialogDescription>
                </DialogHeader>

                <div className="rounded-lg bg-muted p-4 space-y-1">
                    <p className="text-sm font-medium">User Details:</p>
                    <p className="text-sm text-muted-foreground">Name: <span className="font-medium text-foreground">{userName}</span></p>
                    <p className="text-sm text-muted-foreground">Email: <span className="font-medium text-foreground">{userEmail}</span></p>
                </div>

                <div className="rounded-lg border border-yellow-200 bg-yellow-50 p-3">
                    <p className="text-xs text-yellow-800">
                        <strong>Note:</strong> This is a soft delete. The user's data will be preserved but their account will be marked as inactive.
                    </p>
                </div>

                <DialogFooter className="gap-2 sm:gap-0">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={onClose}
                        disabled={deleting}
                    >
                        Cancel
                    </Button>
                    <Button
                        type="button"
                        variant="destructive"
                        onClick={handleConfirm}
                        disabled={deleting}
                    >
                        {deleting ? "Deleting..." : "Delete User"}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
