import { NextResponse } from "next/server";

export async function PATCH(
    request: Request,
    { params }: { params: Promise<{ id: string; imageId: string }> }
) {
    const { id, imageId } = await params;
    const token = request.headers.get("authorization") || "";

    const res = await fetch(`http://127.0.0.1:5000/api/properties/${id}/images/${imageId}/primary`, {
        method: "PATCH",
        headers: token ? { Authorization: token } : {},
    });

    if (!res.ok) {
        const error = await res.json();
        return NextResponse.json(error, { status: res.status });
    }

    return new NextResponse(null, { status: 204 });
}
