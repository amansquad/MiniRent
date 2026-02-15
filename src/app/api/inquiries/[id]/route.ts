import { NextResponse } from "next/server";

export async function PUT(
    request: Request,
    { params }: { params: Promise<{ id: string }> }
) {
    const { id } = await params;
    const token = request.headers.get("authorization") || "";
    const body = await request.json();

    const res = await fetch(`http://127.0.0.1:5000/api/inquiries/${id}`, {
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
        let errorData;
        try {
            errorData = JSON.parse(errorText);
        } catch {
            errorData = { message: errorText };
        }
        return NextResponse.json(
            { error: errorData.message || errorData.title || "Failed to update inquiry", details: errorData },
            { status: res.status }
        );
    }

    const data = await res.json();
    return NextResponse.json(data);
}

export async function DELETE(
    request: Request,
    { params }: { params: Promise<{ id: string }> }
) {
    const { id } = await params;
    const token = request.headers.get("authorization") || "";

    const res = await fetch(`http://127.0.0.1:5000/api/inquiries/${id}`, {
        method: "DELETE",
        headers: {
            ...(token ? { "Authorization": token } : {})
        }
    });

    if (!res.ok) {
        return NextResponse.json({ error: "Failed to delete inquiry" }, { status: res.status });
    }

    return new Response(null, { status: 204 });
}
