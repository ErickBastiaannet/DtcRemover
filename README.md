# DtcRemover

This program is used to delete DTC codes in various software versions. There is also a possibility to remove lists of P-codes. Samples of formats are in the DtcList folder.

# Supported variants
The variants which are supported are also listed in the folder SupportedSoftwareVariants. For new the versions are:

Tested for correct working:
-  04E906027JT_4145
-  4G0907589F_0004
-  04L906026AD_1970
-  8K5907401T_0001
-  4G2907311C_0007
-  8W0907311B_0003
-  4G0906560_0013
-  4K0907401K_0003
-  982906033T_0001
-  4K0907557D_0002
-  4K0907557R_0001
-  4ML907589C_0001
-  04E906027HB_4418
-  8P0907115BA_0020
-  04C906025F_6355
-  04E906016DE_9022
-  04C906026_3537
-  05L906023AD_1067
-  7P0907401_0006

Not yet tested:
-  05E906012Q_1319 (NotConfirmedYet)
-  2G0907115_0004 (NotConfirmedYet)
-  81A907115A_0001 (NotConfirmedYet)
-  8W0907115Q_0003 (NotConfirmedYet)

# Add softwarevariants
It's pretty easy to add a different software variant. You need the start of the 3 code blocks and the length of the code blocks in 8 bit (lengthErrorCodes8bit). In a Damos/A2L file these are named:
-   DFES_DTCO.DFC_Unused_C
-   DFES_Cls.DFC_Unused_C
-   DFC_DisblMsk2.DFC_Unused_C

Then copy the block after "//Add 4G0907589F_0004" till he next comment and replace the values with the correct one for your software variant. Please also add this to this repository.

# What needs work
-  The quick fix : System.Windows.Forms.Application.Restart(); needs to be cleaned.
-  The function of deleting a dtc needs to be properly confirmed by the software, now it's not correctly handled.
