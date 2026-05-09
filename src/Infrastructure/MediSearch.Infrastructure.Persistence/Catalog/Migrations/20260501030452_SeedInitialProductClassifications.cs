using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSearch.Infrastructure.Persistence.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialProductClassifications : Migration
    {
        private static readonly (string Classification, string[] Categories)[] SeedData =
        [
            (
                "Analgesia y Antiinflamatorios",
                [
                    "Analgésicos",
                    "Antiinflamatorios",
                    "Antipiréticos",
                    "Anestésicos locales",
                    "Antirreumáticos",
                    "Otros",
                ]
            ),
            (
                "Antimicrobianos",
                [
                    "Antibióticos",
                    "Antivirales",
                    "Antifúngicos",
                    "Antiparasitarios",
                    "Antisépticos",
                    "Otros",
                ]
            ),
            (
                "Sistema Gastrointestinal",
                [
                    "Antidiarreicos",
                    "Antieméticos",
                    "Laxantes",
                    "Protectores gástricos",
                    "Reguladores intestinales",
                    "Antiácidos",
                    "Antiespasmódicos",
                    "Antiflatulentos",
                    "Otros",
                ]
            ),
            (
                "Sistema Respiratorio",
                [
                    "Antitusivos",
                    "Expectorantes",
                    "Descongestionantes",
                    "Broncodilatadores",
                    "Mucolíticos",
                    "Antiasmáticos",
                    "Alivio del sistema respiratorio",
                    "Antihistamínicos",
                    "Corticosteroides Inhalados",
                    "Antagonistas de Leucotrienos",
                    "Agentes para la Tos",
                    "Otros",
                ]
            ),
            (
                "Cuidado Cardiovascular",
                [
                    "Cardiotónicos",
                    "Antihipertensivos",
                    "Antidiabéticos",
                    "Anticoagulantes",
                    "Antiarrítmicos",
                    "Bienestar cardiovascular",
                    "Diuréticos",
                    "Vasodilatadores",
                    "Estatinas",
                    "Antianginosos",
                    "Antiagregantes plaquetarios",
                    "Betabloqueadores",
                    "Otros",
                ]
            ),
            (
                "Salud Mental",
                [
                    "Antipsicóticos",
                    "Psicoestimulantes",
                    "Ansiolíticos",
                    "Antidepresivos",
                    "Neurolépticos",
                    "Antiepilépticos",
                    "Estabilizadores del ánimo",
                    "Tratamientos para trastornos del sueño",
                    "Antimaníacos",
                    "Otros",
                ]
            ),
            (
                "Cuidado Dermatológico",
                [
                    "Antiacné",
                    "Antipiojos",
                    "Cicatrizantes",
                    "Hidratantes",
                    "Antioxidantes",
                    "Cuidado de la piel",
                    "Cuidado del cabello",
                    "Cuidado solar",
                    "Exfoliantes",
                    "Despigmentantes",
                    "Antiarrugas",
                    "Antiirritantes",
                    "Limpiadores faciales",
                    "Tratamiento de psoriasis",
                    "Tratamiento de eczema",
                    "Protectores labiales",
                    "Tratamiento de dermatitis",
                    "Cremas para pieles sensibles",
                    "Tratamiento de verrugas",
                    "Tratamiento de hongos en la piel",
                    "Productos para contorno de ojos",
                    "Aceites para la piel",
                    "Otros",
                ]
            ),
            (
                "Cuidado Personal",
                [
                    "Repelentes",
                    "Cuidado auditivo",
                    "Cuidado bucal",
                    "Cuidado ocular",
                    "Cuidado nasal",
                    "Higiene corporal",
                    "Desodorantes y antitranspirantes",
                    "Protección solar",
                    "Otros",
                ]
            ),
            (
                "Sistema Nervioso Central",
                [
                    "Sedantes",
                    "Inmunosupresores",
                    "Neuroprotectores",
                    "Anticonvulsivos",
                    "Antiparkinsonianos",
                    "Otros",
                ]
            ),
            (
                "Salud Femenina",
                [
                    "Salud Reproductiva",
                    "Anticoncepción",
                    "Menopausia y Perimenopausia",
                    "Higiene Femenina",
                    "Enfermedades Ginecológicas",
                    "Suplementos prenatales",
                    "Accesorios para la Salud Femenina",
                    "Productos Postparto",
                    "Pruebas de embarazo y ovulación",
                    "Otros",
                ]
            ),
            (
                "Salud sexual",
                [
                    "Anticonceptivos",
                    "Preservativos",
                    "Lubricantes íntimos",
                    "Estimulantes sexuales",
                    "Suplementos para la fertilidad",
                    "Otros",
                ]
            ),
            (
                "Suplementos y Vitaminas",
                [
                    "Multivitamínicos",
                    "Minerales",
                    "Ácidos grasos esenciales",
                    "Proteínas y Aminoácidos",
                    "Suplementos Deportivos",
                    "Energizantes naturales",
                    "Complejos vitamínicos",
                    "Ácidos Grasos y Omega-3",
                    "Otros",
                ]
            ),
            (
                "Otros Productos Farmacéuticos",
                [
                    "Medicamentos Específicos",
                    "Suplementos Adicionales",
                    "Productos para el Cuidado Especializado",
                    "Productos Naturales",
                    "Productos para la Salud General",
                    "Otros",
                ]
            ),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DateTimeOffset seedTimestamp = new(2026, 4, 27, 0, 0, 0, TimeSpan.Zero);

            foreach (var (classificationName, categories) in SeedData)
            {
                string normalizedClassificationName = classificationName.ToLowerInvariant();

                Guid classificationId = CreateDeterministicGuid(
                    $"product-classification:{normalizedClassificationName}"
                );

                migrationBuilder.InsertData(
                    table: "product_classifications",
                    columns:
                    [
                        "id",
                        "name",
                        "created_at",
                        "created_by",
                        "last_modified_at",
                        "last_modified_by",
                    ],
                    values:
                    [
                        classificationId,
                        normalizedClassificationName,
                        seedTimestamp,
                        "SYSTEM",
                        seedTimestamp,
                        "SYSTEM",
                    ]
                );

                foreach (var categoryName in categories)
                {
                    string normalizedCategoryName = categoryName.ToLowerInvariant();

                    migrationBuilder.InsertData(
                        table: "classification_categories",
                        columns:
                        [
                            "id",
                            "classification_id",
                            "name",
                            "created_at",
                            "created_by",
                            "last_modified_at",
                            "last_modified_by",
                        ],
                        values:
                        [
                            CreateDeterministicGuid(
                                $"classification-category:{normalizedClassificationName}:{normalizedCategoryName}"
                            ),
                            classificationId,
                            normalizedCategoryName,
                            seedTimestamp,
                            "SYSTEM",
                            seedTimestamp,
                            "SYSTEM",
                        ]
                    );
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int i = SeedData.Length - 1; i >= 0; i--)
            {
                var (classificationName, categories) = SeedData[i];

                string normalizedClassificationName = classificationName.ToLowerInvariant();

                for (int j = categories.Length - 1; j >= 0; j--)
                {
                    string normalizedCategoryName = categories[j].ToLowerInvariant();

                    migrationBuilder.DeleteData(
                        table: "classification_categories",
                        keyColumn: "id",
                        keyValue: CreateDeterministicGuid(
                            $"classification-category:{normalizedClassificationName}:{normalizedCategoryName}"
                        )
                    );
                }

                migrationBuilder.DeleteData(
                    table: "product_classifications",
                    keyColumn: "id",
                    keyValue: CreateDeterministicGuid(
                        $"product-classification:{normalizedClassificationName}"
                    )
                );
            }
        }

        private static Guid CreateDeterministicGuid(string key)
        {
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(key));

            hash[6] = (byte)((hash[6] & 0x0F) | 0x50);
            hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

            return new Guid(hash);
        }
    }
}
