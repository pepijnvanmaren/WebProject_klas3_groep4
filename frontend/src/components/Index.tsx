import "../styles/index.css";
import React, { useEffect, useState } from "react";
// rawproduct is een typescript type en zorgt voor een goede backend response.
// vraagtekens achter de property name voor nullgeving of verkeerde naamvoering
type RawProduct = {
  ID?: number;
  Naam?: string;
  Foto?: string;
  Beschrijving?: string | null;
  id?: number;
  naam?: string;
  foto?: string | null;
  beschrijving?: string | null;
};
// product entity
type Product = {
  id: number;
  naam?: string | null;
  foto?: string | null;
  beschrijving?: string | null;
};



function Index() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // asynchronische getter voor acceptatiecriteria
  const getProducts = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch("https://localhost:7020/api/Product"); // api naam
      if (!response.ok) {
        const txt = await response.text();
        console.error("Server response:", txt);
        throw new Error("Kon producten niet laden");
      }

      const data: RawProduct[] = await response.json();
      
      // naamgeving 
      const mapped = data.map((p) => ({
        id: p.id ?? p.ID ?? 0,
        naam: p.naam ,
        foto: p.foto ,
        beschrijving: p.beschrijving ,
      })) as Product[];

      setProducts(mapped);
      return response;
    } catch (err: any) {
      console.error("Fout bij ophalen producten:", err);
      if (err && (err as any).message) {
        setError((err as any).message);
      } else {
        setError("Onbekende fout");
      }
      throw err;
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void getProducts();
  }, []);

  return (
    <div className="main_div">
      <div className="content_div" style={{ width: "80%" }}>
        <h2 className="content_div_title">Producten</h2>

        <div style={{ marginBottom: 12, textAlign: "center" }}>
          <button onClick={() => void getProducts()} disabled={loading} className="buy_button" style={{ marginRight: 8 }}>
            {loading ? "Laden..." : "Ververs producten"}
          </button>
        </div>

        {error && <div style={{ color: "red", textAlign: "center" }}>Fout: {error}</div>}

        {products.length === 0 && !loading && !error && <div style={{ textAlign: "center" }}>Geen producten gevonden.</div>}

        <div style={{ display: "flex", flexDirection: "column", gap: "1em", marginTop: "1em" }}>
          {products.map((p) => (
            <div key={p.id} className="content_layout">
              <div className="left_image_div">
                {(p.foto !== undefined && p.foto !== null && p.foto.length > 0) ? (
                  <img src={p.foto} alt={(p.naam !== undefined && p.naam !== null) ? p.naam : `Product ${p.id}`} />
                ) : (
                  <div style={{ color: "#fff" }}>No image</div>
                )}
              </div>

              <div className="right_text_div">
                <h3 style={{ marginTop: 0 }}>{(p.naam !== undefined && p.naam !== null) ? p.naam : `Product #${p.id}`}</h3>
                <div className="bottom_text_div">
                  {(p.beschrijving !== undefined && p.beschrijving !== null) ? p.beschrijving : "Geen beschrijving"}
                </div>

                <div className="price_buy_container" style={{ marginTop: 12 }}>
                  <div style={{ color: "#333", fontWeight: 600 }}>test</div>
                  <button className="buy_button" style={{ marginLeft: 12 }}>Koop</button>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default Index;
