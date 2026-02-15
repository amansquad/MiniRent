import { NextResponse } from "next/server";

export async function PUT(
    request: Request,
    { params }: { params: Promise<{ id: string }> }
) {
    const { id } = await params;
    const token = request.headers.get("authorization") || "";
    const body = await request.json();

    const res = await fetch(`http://127.0.0.1:5000/api/rentals/${id}/status`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        },
        body: JSON.stringify(body)
    });

    if (!res.ok) {
        const errorText = await res.text().catch(() => "Unknown error");
        console.error(`Backend error (${res.status}): ${errorText}`);
        return NextResponse.json(
            { error: "Failed to update rental status" },
            { status: res.status }
        );
    }

    const data = await res.json();
    return NextResponse.json(data);
}
